import { createTorrentEntry, checkAndUpdateTorrent } from './torrentEntries.js';
import {getTrackers} from "./trackerService.js";
import { TorrentType } from './types.js';
import {logger} from "./logger.js";

export async function processTorrentRecord(torrent) {
  const {category} = torrent;
  const type = category === 'tv' ? TorrentType.SERIES : TorrentType.MOVIE;
  const torrentInfo = await parseTorrent(torrent, type);
  logger.info(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`)

  if (await checkAndUpdateTorrent(torrentInfo)) {
    return torrentInfo;
  }

  return createTorrentEntry(torrentInfo);
}

async function assignTorrentTrackers() {
    const trackers = await getTrackers();
    return trackers.join(',');
}

async function parseTorrent(torrent, category) {
  const infoHash = torrent.infoHash?.trim().toLowerCase()
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

function parseImdbId(torrent) {
  if (torrent.imdb === undefined || torrent.imdb === null) {
    return undefined;
  }
  
  return torrent.imdb;
}