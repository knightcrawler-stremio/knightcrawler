export interface ITrackerService {
    getTrackers(): Promise<string[]>;
}