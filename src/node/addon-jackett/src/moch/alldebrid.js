import AllDebridClient from 'all-debrid-api';
import { isVideo, isArchive } from '../lib/extension.js';
import { getMagnetLink } from '../lib/magnetHelper.js';
import { Type } from '../lib/types.js';
import { BadTokenError, AccessDeniedError, sameFilename } from './mochHelper.js';
import StaticResponse from './static.js';

const KEY = 'alldebrid';
const AGENT = 'torrentio';

export async function getCachedStreams(streams, apiKey) {
  const options = await getDefaultOptions();
  const AD = new AllDebridClient(apiKey, options);
  const hashes = streams.map(stream => stream.infohash);
  const available = await AD.magnet.instant(hashes)
      .catch(error => {
        if (toCommonError(error)) {
          return Promise.reject(error);
        }
        console.warn(`Failed AllDebrid cached [${hashes[0]}] torrent availability request:`, error);
        return undefined;
      });
  return available?.data?.magnets && streams
      .reduce((mochStreams, stream) => {
        const cachedEntry = available.data.magnets.find(magnet => stream.infohash === magnet.hash.toLowerCase());
        const streamTitleParts = stream.title.replace(/\n👤.*/s, '').split('\n');
        const fileName = streamTitleParts[streamTitleParts.length - 1];
        const fileIndex = streamTitleParts.length === 2 ? stream.fileIdx : null;
        const encodedFileName = encodeURIComponent(fileName);
        mochStreams[stream.infohash] = {
          url: `${apiKey}/${stream.infohash}/${encodedFileName}/${fileIndex}`,
          cached: cachedEntry?.instant
        }
        return mochStreams;
      }, {})
}

export async function getCatalog(apiKey, offset = 0) {
  if (offset > 0) {
    return [];
  }
  const options = await getDefaultOptions();
  const AD = new AllDebridClient(apiKey, options);
  return AD.magnet.status()
      .then(response => response.data.magnets)
      .then(torrents => (torrents || [])
          .filter(torrent => torrent && statusReady(torrent.statusCode))
          .map(torrent => ({
            id: `${KEY}:${torrent.id}`,
            type: Type.OTHER,
            name: torrent.filename
          })));
}

export async function getItemMeta(itemId, apiKey) {
  const options = await getDefaultOptions();
  const AD = new AllDebridClient(apiKey, options);
  return AD.magnet.status(itemId)
      .then(response => response.data.magnets)
      .then(torrent => ({
        id: `${KEY}:${torrent.id}`,
        type: Type.OTHER,
        name: torrent.filename,
        infohash: torrent.hash.toLowerCase(),
        videos: torrent.links
            .filter(file => isVideo(file.filename))
            .map((file, index) => ({
              id: `${KEY}:${torrent.id}:${index}`,
              title: file.filename,
              released: new Date(torrent.uploadDate * 1000 - index).toISOString(),
              streams: [{ url: `${apiKey}/${torrent.hash.toLowerCase()}/${encodeURIComponent(file.filename)}/${index}` }]
            }))
      }))
}

export async function resolve({ ip, apiKey, infohash, cachedEntryInfo, fileIndex }) {
  console.log(`Unrestricting AllDebrid ${infohash} [${fileIndex}]`);
  const options = await getDefaultOptions(ip);
  const AD = new AllDebridClient(apiKey, options);

  return _resolve(AD, infohash, cachedEntryInfo, fileIndex)
      .catch(error => {
        if (errorExpiredSubscriptionError(error)) {
          console.log(`Access denied to AllDebrid ${infohash} [${fileIndex}]`);
          return StaticResponse.FAILED_ACCESS;
        } else if (error.code === 'MAGNET_TOO_MANY') {
          console.log(`Deleting and retrying adding to AllDebrid ${infohash} [${fileIndex}]...`);
          return _deleteAndRetry(AD, infohash, cachedEntryInfo, fileIndex);
        }
        return Promise.reject(`Failed AllDebrid adding torrent ${JSON.stringify(error)}`);
      });
}

async function _resolve(AD, infohash, cachedEntryInfo, fileIndex) {
  const torrent = await _createOrFindTorrent(AD, infohash);
  if (torrent && statusReady(torrent.statusCode)) {
    return _unrestrictLink(AD, torrent, cachedEntryInfo, fileIndex);
  } else if (torrent && statusDownloading(torrent.statusCode)) {
    console.log(`Downloading to AllDebrid ${infohash} [${fileIndex}]...`);
    return StaticResponse.DOWNLOADING;
  } else if (torrent && statusHandledError(torrent.statusCode)) {
    console.log(`Retrying downloading to AllDebrid ${infohash} [${fileIndex}]...`);
    return _retryCreateTorrent(AD, infohash, cachedEntryInfo, fileIndex);
  }

  return Promise.reject(`Failed AllDebrid adding torrent ${JSON.stringify(torrent)}`);
}

async function _createOrFindTorrent(AD, infohash) {
  return _findTorrent(AD, infohash)
      .catch(() => _createTorrent(AD, infohash));
}

async function _retryCreateTorrent(AD, infohash, encodedFileName, fileIndex) {
  const newTorrent = await _createTorrent(AD, infohash);
  return newTorrent && statusReady(newTorrent.statusCode)
      ? _unrestrictLink(AD, newTorrent, encodedFileName, fileIndex)
      : StaticResponse.FAILED_DOWNLOAD;
}

async function _deleteAndRetry(AD, infohash, encodedFileName, fileIndex) {
  const torrents = await AD.magnet.status().then(response => response.data.magnets);
  const lastTorrent = torrents[torrents.length - 1];
  return AD.magnet.delete(lastTorrent.id)
      .then(() => _retryCreateTorrent(AD, infohash, encodedFileName, fileIndex));
}

async function _findTorrent(AD, infohash) {
  const torrents = await AD.magnet.status().then(response => response.data.magnets);
  const foundTorrents = torrents.filter(torrent => torrent.hash.toLowerCase() === infohash);
  const nonFailedTorrent = foundTorrents.find(torrent => !statusError(torrent.statusCode));
  const foundTorrent = nonFailedTorrent || foundTorrents[0];
  return foundTorrent || Promise.reject('No recent torrent found');
}

async function _createTorrent(AD, infohash) {
  const magnetLink = await getMagnetLink(infohash);
  const uploadResponse = await AD.magnet.upload(magnetLink);
  const torrentId = uploadResponse.data.magnets[0].id;
  return AD.magnet.status(torrentId).then(statusResponse => statusResponse.data.magnets);
}

async function _unrestrictLink(AD, torrent, encodedFileName, fileIndex) {
  const targetFileName = decodeURIComponent(encodedFileName);
  const videos = torrent.links.filter(link => isVideo(link.filename));
  const targetVideo = Number.isInteger(fileIndex)
      ? videos.find(video => sameFilename(targetFileName, video.filename))
      : videos.sort((a, b) => b.size - a.size)[0];

  if (!targetVideo && torrent.links.every(link => isArchive(link.filename))) {
    console.log(`Only AllDebrid archive is available for [${torrent.hash}] ${encodedFileName}`)
    return StaticResponse.FAILED_RAR;
  }
  if (!targetVideo || !targetVideo.link || !targetVideo.link.length) {
    return Promise.reject(`No AllDebrid links found for [${torrent.hash}] ${encodedFileName}`);
  }
  const unrestrictedLink = await AD.link.unlock(targetVideo.link).then(response => response.data.link);
  console.log(`Unrestricted AllDebrid ${torrent.hash} [${fileIndex}] to ${unrestrictedLink}`);
  return unrestrictedLink;
}

async function getDefaultOptions(ip) {
  return { base_agent: AGENT, timeout: 10000 };
}

export function toCommonError(error) {
  if (error && error.code === 'AUTH_BAD_APIKEY') {
    return BadTokenError;
  }
  if (error && error.code === 'AUTH_USER_BANNED') {
    return AccessDeniedError;
  }
  return undefined;
}

function statusError(statusCode) {
  return [5, 6, 7, 8, 9, 10, 11].includes(statusCode);
}

function statusHandledError(statusCode) {
  return [5, 7, 9, 10, 11].includes(statusCode);
}

function statusDownloading(statusCode) {
  return [0, 1, 2, 3].includes(statusCode);
}

function statusReady(statusCode) {
  return statusCode === 4;
}

function errorExpiredSubscriptionError(error) {
  return ['AUTH_BAD_APIKEY', 'MUST_BE_PREMIUM', 'MAGNET_MUST_BE_PREMIUM', 'FREE_TRIAL_LIMIT_REACHED', 'AUTH_USER_BANNED']
      .includes(error.code);
}
