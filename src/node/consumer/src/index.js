import { listenToQueue } from './jobs/processTorrents';
import { repository } from "./repository/database_repository";
import { trackerService } from "./lib/services/tracker_service";

(async () => {
    await trackerService.getTrackers();
    await repository.connect();
    await listenToQueue();
})();