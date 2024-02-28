import {IProcessTorrentsJob} from "@interfaces/process_torrents_job";
import {ITrackerService} from "@interfaces/tracker_service";
import {IMongoRepository} from "@mongo/interfaces/mongo_repository";
import {IDatabaseRepository} from "@repository/interfaces/database_repository";
import {IocTypes} from "@setup/ioc_types";
import {inject, injectable} from "inversify";

export interface ICompositionalRoot {
    start(): Promise<void>;
}

@injectable()
export class CompositionalRoot implements ICompositionalRoot {
    @inject(IocTypes.ITrackerService) trackerService: ITrackerService;
    @inject(IocTypes.IDatabaseRepository) databaseRepository: IDatabaseRepository;
    @inject(IocTypes.IMongoRepository) mongoRepository: IMongoRepository;
    @inject(IocTypes.IProcessTorrentsJob) processTorrentsJob: IProcessTorrentsJob;

    async start(): Promise<void> {
        await this.trackerService.getTrackers();
        await this.databaseRepository.connect();
        await this.mongoRepository.connect();
        await this.processTorrentsJob.listenToQueue();
    }
}