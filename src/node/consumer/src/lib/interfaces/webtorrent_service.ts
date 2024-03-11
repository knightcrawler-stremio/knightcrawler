import {ITorrentFile} from "@interfaces/torrent_file";

export interface IWebTorrentService {
    getTorrentContents(magnet: string, timeoutWindow: number): Promise<ITorrentFile[]>
}
