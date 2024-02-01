import { getTrackers } from "./lib/trackerService.js";
import { connect } from './lib/repository.js';
import { listenToQueue } from './jobs/processTorrents.js';
import { jobConfig } from "./lib/config.js";

await getTrackers();
await connect();

if (jobConfig.JOBS_ENABLED) {
    await listenToQueue();
}