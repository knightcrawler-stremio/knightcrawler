import magnet from 'magnet-uri';
import OffcloudClient from 'offcloud-api';
import { isVideo } from '../lib/extension.js';
import { getMagnetLink } from '../lib/magnetHelper.js';
import { Type } from '../lib/types.js';
import { chunkArray, BadTokenError, sameFilename } from './mochHelper.js';
import StaticResponse from './static.js';

const KEY = 'offcloud';

export async function getCachedStreams(streams, apiKey) {
  const options = await getDefaultOptions();
  const OC = new OffcloudClient(apiKey, options);
  const hashBatches = chunkArray(streams.map(stream => stream.infohash), 100);
  const available = await Promise.all(hashBatches.map(hashes => OC.instant.cache(hashes)))
      .then(results => results.map(result => result.cachedItems))
      .then(results => results.reduce((all, result) => all.concat(result), []))
      .catch(error => {
        if (toCommonError(error)) {
          return Promise.reject(error);
        }
        console.warn('Failed Offcloud cached torrent availability request:', error);
        return undefined;
      });
  return available && streams
      .reduce((mochStreams, stream) => {
        const isCached = available.includes(stream.infohash);
        const streamTitleParts = stream.title.replace(/\n👤.*/s, '').split('\n');
        const fileName = streamTitleParts[streamTitleParts.length - 1];
        const fileIndex = streamTitleParts.length === 2 ? stream.fileIdx : null;
        const encodedFileName = encodeURIComponent(fileName);
        mochStreams[stream.infohash] = {
          url: `${apiKey}/${stream.infohash}/${encodedFileName}/${fileIndex}`,
          cached: isCached
        };
        return mochStreams;
      }, {})
}

export async function getCatalog(apiKey, offset = 0) {
  if (offset > 0) {
    return [];
  }
  const options = await getDefaultOptions();
  const OC = new OffcloudClient(apiKey, options);
  return OC.cloud.history()
      .then(torrents => torrents)
      .then(torrents => (torrents || [])
          .map(torrent => ({
            id: `${KEY}:${torrent.requestId}`,
            type: Type.OTHER,
            name: torrent.fileName
          })));
}

export async function getItemMeta(itemId, apiKey, ip) {
  const options = await getDefaultOptions(ip);
  const OC = new OffcloudClient(apiKey, options);
  const torrents = await OC.cloud.history();
  const torrent = torrents.find(torrent => torrent.requestId === itemId)
  const infohash = torrent && magnet.decode(torrent.originalLink).infohash
  const createDate = torrent ? new Date(torrent.createdOn) : new Date();
  return _getFileUrls(OC, torrent)
      .then(files => ({
        id: `${KEY}:${itemId}`,
        type: Type.OTHER,
        name: torrent.name,
        infohash: infohash,
        videos: files
            .filter(file => isVideo(file))
            .map((file, index) => ({
              id: `${KEY}:${itemId}:${index}`,
              title: file.split('/').pop(),
              released: new Date(createDate.getTime() - index).toISOString(),
              streams: [{ url: file }]
            }))
      }))
}

export async function resolve({ ip, apiKey, infohash, cachedEntryInfo, fileIndex }) {
  console.log(`Unrestricting Offcloud ${infohash} [${fileIndex}]`);
  const options = await getDefaultOptions(ip);
  const OC = new OffcloudClient(apiKey, options);

  return _resolve(OC, infohash, cachedEntryInfo, fileIndex)
      .catch(error => {
        if (errorExpiredSubscriptionError(error)) {
          console.log(`Access denied to Offcloud ${infohash} [${fileIndex}]`);
          return StaticResponse.FAILED_ACCESS;
        }
        return Promise.reject(`Failed Offcloud adding torrent ${JSON.stringify(error)}`);
      });
}

async function _resolve(OC, infohash, cachedEntryInfo, fileIndex) {
  const torrent = await _createOrFindTorrent(OC, infohash)
      .then(info => info.requestId ? OC.cloud.status(info.requestId) : Promise.resolve(info))
      .then(info => info.status || info);
  if (torrent && statusReady(torrent)) {
    return _unrestrictLink(OC, infohash, torrent, cachedEntryInfo, fileIndex);
  } else if (torrent && statusDownloading(torrent)) {
    console.log(`Downloading to Offcloud ${infohash} [${fileIndex}]...`);
    return StaticResponse.DOWNLOADING;
  } else if (torrent && statusError(torrent)) {
    console.log(`Retry failed download in Offcloud ${infohash} [${fileIndex}]...`);
    return _retryCreateTorrent(OC, infohash, cachedEntryInfo, fileIndex);
  }

  return Promise.reject(`Failed Offcloud adding torrent ${JSON.stringify(torrent)}`);
}

async function _createOrFindTorrent(OC, infohash) {
  return _findTorrent(OC, infohash)
      .catch(() => _createTorrent(OC, infohash));
}

async function _findTorrent(OC, infohash) {
  const torrents = await OC.cloud.history();
  const foundTorrents = torrents.filter(torrent => torrent.originalLink.toLowerCase().includes(infohash));
  const nonFailedTorrent = foundTorrents.find(torrent => !statusError(torrent));
  const foundTorrent = nonFailedTorrent || foundTorrents[0];
  return foundTorrent || Promise.reject('No recent torrent found');
}

async function _createTorrent(OC, infohash) {
  const magnetLink = await getMagnetLink(infohash);
  return OC.cloud.download(magnetLink)
}

async function _retryCreateTorrent(OC, infohash, cachedEntryInfo, fileIndex) {
  const newTorrent = await _createTorrent(OC, infohash);
  return newTorrent && statusReady(newTorrent.status)
      ? _unrestrictLink(OC, infohash, newTorrent, cachedEntryInfo, fileIndex)
      : StaticResponse.FAILED_DOWNLOAD;
}

async function _unrestrictLink(OC, infohash, torrent, cachedEntryInfo, fileIndex) {
  const targetFileName = decodeURIComponent(cachedEntryInfo);
  const files = await _getFileUrls(OC, torrent)
  const targetFile = files.find(file => sameFilename(targetFileName, file.split('/').pop()))
      || files.find(file => isVideo(file))
      || files.pop();

  if (!targetFile) {
    return Promise.reject(`No Offcloud links found for index ${fileIndex} in: ${JSON.stringify(torrent)}`);
  }
  console.log(`Unrestricted Offcloud ${infohash} [${fileIndex}] to ${targetFile}`);
  return targetFile;
}

async function _getFileUrls(OC, torrent) {
  return OC.cloud.explore(torrent.requestId)
      .catch(error => {
        if (error === 'Bad archive') {
          return [`https://${torrent.server}.offcloud.com/cloud/download/${torrent.requestId}/${torrent.fileName}`];
        }
        throw error;
      })
}

async function getDefaultOptions(ip) {
  return { ip, timeout: 10000 };
}

export function toCommonError(error) {
  if (error?.error === 'NOAUTH' || error?.message?.startsWith('Cannot read property')) {
    return BadTokenError;
  }
  return undefined;
}

function statusDownloading(torrent) {
  return ['downloading', 'created'].includes(torrent.status);
}

function statusError(torrent) {
  return ['error', 'canceled'].includes(torrent.status);
}

function statusReady(torrent) {
  return torrent.status === 'downloaded';
}

function errorExpiredSubscriptionError(error) {
  return error?.includes && (error.includes('not_available') || error.includes('NOAUTH') || error.includes('premium membership'));
}
