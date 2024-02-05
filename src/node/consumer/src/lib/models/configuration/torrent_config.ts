export class TorrentConfig {
    public MAX_CONNECTIONS_PER_TORRENT: number = parseInt(process.env.MAX_SINGLE_TORRENT_CONNECTIONS || "20", 10);
    public TIMEOUT: number = parseInt(process.env.TORRENT_TIMEOUT || "30000", 10);
}