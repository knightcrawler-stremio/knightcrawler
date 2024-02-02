import axios from 'axios';
import {cacheTrackers} from "./cache.js";
import { trackerConfig } from './config.js';

const downloadTrackers = async () => {
    const response = await axios.get(trackerConfig.TRACKERS_URL);
    const trackersListText = response.data;
    // Trackers are separated by a newline character
    let urlTrackers = trackersListText.split("\n");
    // remove blank lines
    urlTrackers = urlTrackers.filter(line => line.trim() !== '');
    
    if (!trackerConfig.UDP_ENABLED) {
        // remove any udp trackers
        urlTrackers = urlTrackers.filter(line => !line.startsWith('udp://'));    
    }

    console.log(`Trackers updated at ${Date.now()}: ${urlTrackers.length} trackers`);

    return urlTrackers;
};

export const getTrackers = async () => {
    return cacheTrackers(downloadTrackers);
};