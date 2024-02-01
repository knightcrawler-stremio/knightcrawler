import { Type } from './types.js';
import { createTorrentEntry, checkAndUpdateTorrent } from './torrentEntries.js';
import {getTrackers} from "./trackerService.js";

export async function processTorrentRecord(torrent) {
  const category = torrent.category;
  const type = category === 'tv' ? Type.SERIES : Type.MOVIE;
  const torrentInfo = await parseTorrent(torrent, type);
  console.log(`Processing torrent ${torrentInfo.title} with infoHash ${torrentInfo.infoHash}`)

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