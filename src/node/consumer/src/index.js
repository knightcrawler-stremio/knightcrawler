import { listenToQueue } from './jobs/processTorrents.js';
import { repository } from "./repository/database_repository";
import { getTrackers } from "./lib/trackerService.js";

(async () => {
    await getTrackers();
    await repository.connect();
    await listenToQueue();
})();