import axios from "axios";
import {cacheWrapImdbMetaData} from "./cache.js";
import {getRandomUserAgent} from "./requestHelper.js";
import {cinemetaConfig, processConfig} from "./settings.js";

const cinemetaUri = cinemetaConfig.URI;

export const getMetaData = (args) => {
    const [imdbId] = args.id.split(':');
    const {type} = args;
    
    return cacheWrapImdbMetaData(args.id, () => getInfoForImdbId(imdbId, type));
}

const getInfoForImdbId = async (imdbId, type) => {
    const requestUri = `${cinemetaUri}/${type}/${imdbId}.json`;
    const options = { timeout: 30000, headers: { 'User-Agent': getRandomUserAgent() } };

    if (processConfig.DEBUG) {
        console.log(`Getting info for ${imdbId} of type ${type}`);
        console.log(`Request URI: ${requestUri}`);
    }
    
    try {
        const { data: response } = await axios.get(requestUri, options);
        return response.meta;
    } catch (error) {
        console.log(error);
        return {};
    }
};