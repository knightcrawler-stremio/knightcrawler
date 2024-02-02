import axios from 'axios';
import {jackettConfig, processConfig} from "../lib/settings.js";
import {cleanName, transformData} from "./jacketParser.js";
import {jackettSearchQueries} from "./jackettQueries.js";

const JACKETT_SEARCH_URI = `${jackettConfig.URI}/api/v2.0/indexers/!status:failing,test:passed/results/torznab?apikey=${jackettConfig.API_KEY}`;

const performRequest = async (url) => {
    const response = await axios.get(url, { timeout: jackettConfig.TIMEOUT, responseType: 'text' });
    return !response.data ? null : response.data;
};

export const searchJackett = async (query) => {
    if (processConfig.DEBUG) {
        console.log('Beginning jackett query construction for', query);
    }

    const name = encodeURIComponent(cleanName(query.name));

    const queries = jackettSearchQueries(name, query.type, query.year, query.season, query.episode);

    const flatQueries = [].concat(...Object.values(queries));

    const fetchPromises = flatQueries.map(searchQuery => {
        const url = JACKETT_SEARCH_URI + searchQuery;
        return performRequest(url);
    });

    const responses = await Promise.all(fetchPromises);

    let sortedResults = [];

    for (const response of responses) {
        if (response) {
            const transformedData = await transformData(response, query);
            sortedResults = sortedResults.concat(transformedData);
        }
    }

    return sortedResults;
};