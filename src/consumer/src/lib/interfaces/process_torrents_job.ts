export interface IProcessTorrentsJob {
    listenToQueue: () => Promise<void>;
}
