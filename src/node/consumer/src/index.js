import { listenToQueue } from './jobs/processTorrents.js';
import { connect } from './lib/repository.js';
import { getTrackers } from "./lib/trackerService.js";

(async () => {
    await getTrackers();
    await connect();
    await listenToQueue();
})();