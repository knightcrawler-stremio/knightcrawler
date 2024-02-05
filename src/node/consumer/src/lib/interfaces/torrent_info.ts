export interface TorrentInfo {
    title: string | null;
    torrentId: string;
    infoHash: string | null;
    seeders: number;
    size: string | null;
    uploadDate: Date;
    imdbId: string | undefined;
    type: string;
    provider: string | null;
    trackers: string;
}