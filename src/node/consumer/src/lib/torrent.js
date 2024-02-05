import { decode } from 'magnet-uri';
import torrentStream from 'torrent-stream';
import { torrentConfig } from './config';
import {isSubtitle, isVideo} from './extension';

export async function torrentFiles(torrent, timeout) {
  return filesFromTorrentStream(torrent, timeout)
      .then(files => ({
        contents: files,
        videos: filterVideos(files),
        subtitles: filterSubtitles(files)
      }));
}

async function filesFromTorrentStream(torrent, timeout) {
  return filesAndSizeFromTorrentStream(torrent, timeout).then(result => result.files);
}

const engineOptions = {
  connections: torrentConfig.MAX_CONNECTIONS_PER_TORRENT,
  uploads: 0,
  verify: false,
  dht: false,
  tracker: true
}

function filesAndSizeFromTorrentStream(torrent, timeout = 30000) {
  if (!torrent.infoHash) {
    return Promise.reject(new Error("no infoHash..."));
  }
  const magnet = decode.encode({ infoHash: torrent.infoHash, announce: torrent.trackers });
  return new Promise((resolve, rejected) => {
    const timeoutId = setTimeout(() => {
      engine.destroy();
      rejected(new Error('No available connections for torrent!'));
    }, timeout);

    const engine = new torrentStream(magnet, engineOptions);

    engine.ready(() => {
      const files = engine.files
          .map((file, fileId) => ({
            fileIndex: fileId,
            name: file.name,
            path: file.path.replace(/^[^/]+\//, ''),
            size: file.length
          }));
      const size = engine.torrent.length;
      resolve({ files, size });
      engine.destroy();
      clearTimeout(timeoutId);
    });
  });
}

function filterVideos(files) {
  if (files.length === 1 && !Number.isInteger(files[0].fileIndex)) {
    return files;
  }
  const videos = files.filter(file => isVideo(file.path));
  const maxSize = Math.max(...videos.map(video => video.size));
  const minSampleRatio = videos.length <= 3 ? 3 : 10;
  const minAnimeExtraRatio = 5;
  const minRedundantRatio = videos.length <= 3 ? 30 : Number.MAX_VALUE;
  const isSample = video => video.path.match(/sample|bonus|promo/i) && maxSize / parseInt(video.size) > minSampleRatio;
  const isRedundant = video => maxSize / parseInt(video.size) > minRedundantRatio;
  const isExtra = video => video.path.match(/extras?\//i);
  const isAnimeExtra = video => video.path.match(/(?:\b|_)(?:NC)?(?:ED|OP|PV)(?:v?\d\d?)?(?:\b|_)/i)
      && maxSize / parseInt(video.size) > minAnimeExtraRatio;
  const isWatermark = video => video.path.match(/^[A-Z-]+(?:\.[A-Z]+)?\.\w{3,4}$/)
      && maxSize / parseInt(video.size) > minAnimeExtraRatio
 return videos
      .filter(video => !isSample(video))
      .filter(video => !isExtra(video))
      .filter(video => !isAnimeExtra(video))
      .filter(video => !isRedundant(video))
      .filter(video => !isWatermark(video));
}

function filterSubtitles(files) {
  return files.filter(file => isSubtitle(file.path));
}
