import { listenToQueue } from './jobs/processTorrents.js';
import { jobConfig } from "./lib/config.js";
import { connect } from './lib/repository.js';
import { getTrackers } from "./lib/trackerService.js";

(async () => {
    await getTrackers();
    await connect();

    if (jobConfig.JOBS_ENABLED) {
        await listenToQueue();
    }
})();