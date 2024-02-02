import videoNameParser from "video-name-parser";
import {parseStringPromise as parseString} from "xml2js";
import {processConfig, jackettConfig} from "../lib/settings.js";

export function extractSize(title) {
    const seedersMatch = title.match(/💾 ([\d.]+ \w+)/);
    return seedersMatch && parseSize(seedersMatch[1]) || 0;
}

export function parseSize(sizeText) {
    if (!sizeText) {
        return 0;
    }
    let scale = 1;
    if (sizeText.includes('TB')) {
        scale = 1024 * 1024 * 1024 * 1024
    } else if (sizeText.includes('GB')) {
        scale = 1024 * 1024 * 1024
    } else if (sizeText.includes('MB')) {
        scale = 1024 * 1024;
    } else if (sizeText.includes('kB')) {
        scale = 1024;
    }
    return Math.floor(parseFloat(sizeText.replace(/,/g, '')) * scale);
}

export const parseVideo = (name) => {
    return videoNameParser(name + '.mp4');
};

export const episodeTag = (season, episode) => {
    const paddedSeason = season < 10 ? `0${season}` : season;
    const paddedEpisode = episode < 10 ? `0${episode}` : episode;
    return `S${paddedSeason}E${paddedEpisode}`;
};

export const cleanName = (name) => {
    name = name.replace(/[._\-–()\[\]:,]/g, ' ');
    name = name.replace(/\s+/g, ' ');
    name = name.replace(/'/g, '');
    name = name.replace(/\\\\/g, '\\').replace(/\\\\'|\\'|\\\\"|\\"/g, '');
    return name;
};

export const insertIntoSortedArray = (sortedArray, newObject, sortingProperty, maxSize) => {
    const indexToInsert = sortedArray.findIndex(item => item[sortingProperty] < newObject[sortingProperty]);

    if (indexToInsert === -1) {
        if (sortedArray.length < maxSize) {
            sortedArray.push(newObject);
            return true;
        }
        return false;
    } else {
        // Insert the new object at the correct position to maintain the sorted order (descending)
        sortedArray.splice(indexToInsert, 0, newObject);
        // Trim the array if it exceeds maxSize
        if (sortedArray.length > maxSize) {
            sortedArray.pop();
        }
        return true;
    }
};

export const extraTag = (name, searchQuery) => {
    const parsedName = parseVideo(name + '.mp4');
    let extraTag = cleanName(name);
    searchQuery = cleanName(searchQuery);

    extraTag = extraTag.replace(new RegExp(searchQuery, 'gi'), '');
    extraTag = extraTag.replace(new RegExp(parsedName.name, 'gi'), '');

    if (parsedName.year) {
        extraTag = extraTag.replace(parsedName.year.toString(), '');
    }

    if (parsedName.season && parsedName.episode && parsedName.episode.length) {
        extraTag = extraTag.replace(new RegExp(episodeTag(parsedName.season, parsedName.episode[0]), 'gi'), '');
    }

    extraTag = extraTag.trim();

    let extraParts = extraTag.split(' ');

    if (parsedName.season && parsedName.episode && parsedName.episode.length) {
        if (extraParts[0] && extraParts[0].length === 2 && !isNaN(extraParts[0])) {
            const possibleEpTag = `${episodeTag(parsedName.season, parsedName.episode[0])}-${extraParts[0]}`;
            if (name.toLowerCase().includes(possibleEpTag.toLowerCase())) {
                extraParts[0] = possibleEpTag;
            }
        }
    }

    const foundPart = name.toLowerCase().indexOf(extraParts[0].toLowerCase());

    if (foundPart > -1) {
        extraTag = name.substring(foundPart).replace(/[_()\[\],]/g, ' ');

        if ((extraTag.match(/\./g) || []).length > 1) {
            extraTag = extraTag.replace(/\./g, ' ');
        }

        extraTag = extraTag.replace(/\s+/g, ' ');
    }

    return extraTag;
};


export const transformData = async (data, query) => {
    console.log("Transforming data for query " + data);

    let results = [];
    
    const parsedData = await parseString(data);

    if (!parsedData.rss.channel[0]?.item) {
        return [];
    }

    for (const rssItem of parsedData.rss.channel[0].item) {
        let torznabData = {};

        rssItem["torznab:attr"].forEach((torznabDataItem) =>
            Object.assign(torznabData, {
                [torznabDataItem.$.name]: torznabDataItem.$.value,
            })
        );
        
        if (torznabData.infohash) {

            const [title, pubDate, category, size] = [rssItem.title[0], rssItem.pubDate[0], rssItem.category[0], rssItem.size[0]];

            torznabData = {
                ...torznabData,
                title,
                pubDate,
                category,
                size,
                extraTag: extraTag(title, query.name)
            };

            if (insertIntoSortedArray(results, torznabData, 'size', jackettConfig.MAXIMUM_RESULTS)) {
                processConfig.DEBUG && console.log(torznabData);
            }
        }
    }


    return results;
};