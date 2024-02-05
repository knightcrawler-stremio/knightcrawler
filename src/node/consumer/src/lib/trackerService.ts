import axios, { AxiosResponse } from 'axios';
import { cacheTrackers } from "./cache";
import { trackerConfig } from './config';
import { logger } from "./logger";

const downloadTrackers = async (): Promise<string[]> => {
    const response: AxiosResponse<string> = await axios.get(trackerConfig.TRACKERS_URL);
    const trackersListText: string = response.data;
    // Trackers are separated by a newline character
    let urlTrackers = trackersListText.split("\n");
    // remove blank lines
    urlTrackers = urlTrackers.filter(line => line.trim() !== '');

    if (!trackerConfig.UDP_ENABLED) {
        // remove any udp trackers
        urlTrackers = urlTrackers.filter(line => !line.startsWith('udp://'));

    }

    logger.info(`Trackers updated at ${Date.now()}: ${urlTrackers.length} trackers`);

    return urlTrackers;
};

export const getTrackers = async (): Promise<string[]> => {
    return cacheTrackers(downloadTrackers);
};