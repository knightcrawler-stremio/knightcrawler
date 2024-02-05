import {TorrentInfo} from "./interfaces/torrent_info";
import {TorrentType} from "./enums/torrent_types";
import {logger} from "./logger";
import {checkAndUpdateTorrent, createTorrentEntry} from "./torrentEntries.js";
import {getTrackers} from "./trackerService";
import {IngestedTorrentAttributes} from "../repository/interfaces/ingested_torrent_attributes";

export async function processTorrentRecord(torrent: IngestedTorrentAttributes): Promise<void> {
    const { category } = torrent;
    const type = category === 'tv' ? TorrentType.SERIES : TorrentType.MOVIE;
    const torrentInfo: TorrentInfo = await parseTorrent(torrent, type);

    logger.info(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`);

    if (await checkAndUpdateTorrent(torrentInfo)) {
        return;
    }

    return createTorrentEntry(torrentInfo);
}

async function assignTorrentTrackers(): Promise<string> {
    const trackers = await getTrackers();
    return trackers.join(',');
}

async function parseTorrent(torrent: IngestedTorrentAttributes, category: string): Promise<TorrentInfo> {
    const infoHash = torrent.info_hash?.trim().toLowerCase()
    return {
        title: torrent.name,
        torrentId: `${torrent.name}_${infoHash}`,
        infoHash: infoHash,
        seeders: 100,
        size: torrent.size,
        uploadDate: torrent.createdAt,
        imdbId: parseImdbId(torrent),
        type: category,
        provider: torrent.source,
        trackers: await assignTorrentTrackers(),
    }
}

function parseImdbId(torrent: IngestedTorrentAttributes): string | undefined {
    if (torrent.imdb === undefined || torrent.imdb === null) {
        return undefined;
    }

    return torrent.imdb;
}