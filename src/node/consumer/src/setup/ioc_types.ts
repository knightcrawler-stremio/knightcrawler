export const IocTypes = {
    // Composition root
    ICompositionalRoot: Symbol.for("ICompositionalRoot"),
    // Services
    ICacheService: Symbol.for("ICacheService"),
    ILoggingService: Symbol.for("ILoggingService"),
    IMetadataService: Symbol.for("IMetadataService"),
    ITorrentDownloadService: Symbol.for("ITorrentDownloadService"),
    ITorrentEntriesService: Symbol.for("ITorrentEntriesService"),
    ITorrentFileService: Symbol.for("ITorrentFileService"),
    ITorrentProcessingService: Symbol.for("ITorrentProcessingService"),
    ITorrentSubtitleService: Symbol.for("ITorrentSubtitleService"),
    ITrackerService: Symbol.for("ITrackerService"),
    // DAL
    IDatabaseRepository: Symbol.for("IDatabaseRepository"),
    // Jobs
    IProcessTorrentsJob: Symbol.for("IProcessTorrentsJob"),
};