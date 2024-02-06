import { processTorrentsJob } from './jobs/process_torrents_job.js';
import { repository } from "./repository/database_repository";
import { trackerService } from "./lib/services/tracker_service";

(async () => {
    await trackerService.getTrackers();
    await repository.connect();
    await processTorrentsJob.listenToQueue();
})();