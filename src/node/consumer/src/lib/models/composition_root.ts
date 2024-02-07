import {inject, injectable} from "inversify";
import {IDatabaseRepository} from "../../repository/interfaces/database_repository";
import {ITrackerService} from "../interfaces/tracker_service";
import {IProcessTorrentsJob} from "../../interfaces/process_torrents_job";
import {ICompositionalRoot} from "../interfaces/composition_root";
import {IocTypes} from "./ioc_types";

@injectable()
export class CompositionalRoot implements ICompositionalRoot {
    private trackerService: ITrackerService;
    private databaseRepository: IDatabaseRepository;
    private processTorrentsJob: IProcessTorrentsJob;
    constructor(@inject(IocTypes.ITrackerService) trackerService: ITrackerService,
                @inject(IocTypes.IDatabaseRepository) databaseRepository: IDatabaseRepository,
                @inject(IocTypes.IProcessTorrentsJob) processTorrentsJob: IProcessTorrentsJob) {
        this.trackerService = trackerService;
        this.databaseRepository = databaseRepository;
        this.processTorrentsJob = processTorrentsJob;
    }

    start = async () => {
        await this.trackerService.getTrackers();
        await this.databaseRepository.connect();
        await this.processTorrentsJob.listenToQueue();
    };
}