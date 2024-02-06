import { parse } from 'parse-torrent-title';
import { repository } from '../repository/database_repository';
import { TorrentType } from './enums/torrent_types';
import { PromiseHelpers } from './helpers/promises_helpers';
import { logger } from './services/logging_service';
import { metadataService } from './services/metadata_service';
import { parsingService } from './services/parsing_service';
import { torrentFileService } from './services/torrent_file_service';
import { torrentSubtitleService } from './services/torrent_subtitle_service';

export async function createTorrentEntry(torrent, overwrite = false)  {
  const titleInfo = parse(torrent.title);
    
  if (!torrent.imdbId && torrent.type !== TorrentType.Anime) {
      const imdbQuery = {
          title: titleInfo.title,
          year: titleInfo.year,
          type: torrent.type
      };
      torrent.imdbId = await metadataService.getImdbId(imdbQuery)
        .catch(() => undefined);
  }
  if (torrent.imdbId && torrent.imdbId.length < 9) {
    // pad zeros to imdbId if missing
    torrent.imdbId = 'tt' + torrent.imdbId.replace('tt', '').padStart(7, '0');
  }
  if (torrent.imdbId && torrent.imdbId.length > 9 && torrent.imdbId.startsWith('tt0')) {
    // sanitize imdbId from redundant zeros
    torrent.imdbId = torrent.imdbId.replace(/tt0+([0-9]{7,})$/, 'tt$1');
  }
  if (!torrent.kitsuId && torrent.type === TorrentType.Anime) {
      const kitsuQuery = {
          title: titleInfo.title,
          year: titleInfo.year,
          season: titleInfo.season,
      };
      torrent.kitsuId = await metadataService.getKitsuId(kitsuQuery)
        .catch(() => undefined);
  }

  if (!torrent.imdbId && !torrent.kitsuId && !parsingService.isPackTorrent(torrent)) {
    logger.warn(`imdbId or kitsuId not found:  ${torrent.provider} ${torrent.title}`);
    return;
  }

  const { contents, videos, subtitles } = await torrentFileService.parseTorrentFiles(torrent)
      .then(torrentContents => overwrite ? overwriteExistingFiles(torrent, torrentContents) : torrentContents)
      .then(torrentContents => torrentSubtitleService.assignSubtitles(torrentContents))
      .catch(error => {
          logger.warn(`Failed getting files for ${torrent.title}`, error.message);
        return {};
      });
  if (!videos || !videos.length) {
      logger.warn(`no video files found for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`);
    return;
  }

  return repository.createTorrent({ ...torrent, contents, subtitles })
      .then(() => PromiseHelpers.sequence(videos.map(video => () => repository.createFile(video))))
      .then(() => logger.info(`Created ${torrent.provider} entry for [${torrent.infoHash}] ${torrent.title}`));
}

async function overwriteExistingFiles(torrent, torrentContents) {
  const videos = torrentContents && torrentContents.videos;
  if (videos && videos.length) {
    const existingFiles = await repository.getFiles({ infoHash: videos[0].infoHash })
        .then((existing) => existing
            .reduce((map, next) => {
              const fileIndex = next.fileIndex !== undefined ? next.fileIndex : null;
              map[fileIndex] = (map[fileIndex] || []).concat(next);
              return map;
            }, {}))
        .catch(() => undefined);
    if (existingFiles && Object.keys(existingFiles).length) {
      const overwrittenVideos = videos
          .map(file => {
            const mapping = videos.length === 1 && Object.keys(existingFiles).length === 1
                ? Object.values(existingFiles)[0]
                : existingFiles[file.fileIndex !== undefined ? file.fileIndex : null];
            if (mapping) {
              const originalFile = mapping.shift();
              return { id: originalFile.id, ...file };
            }
            return file;
          });
      return { ...torrentContents, videos: overwrittenVideos };
    }
    return torrentContents;
  }
  return Promise.reject(`No video files found for: ${torrent.title}`);
}

export async function createSkipTorrentEntry(torrent) {
  return repository.createSkipTorrent(torrent);
}

export async function getStoredTorrentEntry(torrent) {
  return repository.getSkipTorrent(torrent)
      .catch(() => repository.getTorrent(torrent))
      .catch(() => undefined);
}

export async function checkAndUpdateTorrent(torrent) {
  const storedTorrent = torrent.dataValues
      ? torrent
      : await repository.getTorrent(torrent).catch(() => undefined);
  if (!storedTorrent) {
    return false;
  }
  if (storedTorrent.provider === 'RARBG') {
    return true;
  }
  if (storedTorrent.provider === 'KickassTorrents' && torrent.provider) {
    storedTorrent.provider = torrent.provider;
    storedTorrent.torrentId = torrent.torrentId;
  }
  if (!storedTorrent.languages && torrent.languages && storedTorrent.provider !== 'RARBG') {
    storedTorrent.languages = torrent.languages;
    await storedTorrent.save();
    logger.debug(`Updated [${storedTorrent.infoHash}] ${storedTorrent.title} language to ${torrent.languages}`);
  }
  return createTorrentContents({ ...storedTorrent.get(), torrentLink: torrent.torrentLink })
      .then(() => updateTorrentSeeders(torrent));
}

export async function createTorrentContents(torrent) {
  if (torrent.opened) {
    return;
  }
  const storedVideos = await repository.getFiles(torrent).catch(() => []);
  if (!storedVideos || !storedVideos.length) {
    return;
  }
  const notOpenedVideo = storedVideos.length === 1 && !Number.isInteger(storedVideos[0].fileIndex);
  const imdbId = PromiseHelpers.mostCommonValue(storedVideos.map(stored => stored.imdbId));
  const kitsuId = PromiseHelpers.mostCommonValue(storedVideos.map(stored => stored.kitsuId));

  const { contents, videos, subtitles } = await torrentFileService.parseTorrentFiles({ ...torrent, imdbId, kitsuId })
      .then(torrentContents => notOpenedVideo ? torrentContents : { ...torrentContents, videos: storedVideos })
      .then(torrentContents => torrentSubtitleService.assignSubtitles(torrentContents))
      .catch(error => {
        logger.warn(`Failed getting contents for [${torrent.infoHash}] ${torrent.title}`, error.message);
        return {};
      });

  if (!contents || !contents.length) {
    return;
  }
  if (notOpenedVideo && videos.length === 1) {
    // if both have a single video and stored one was not opened, update stored one to true metadata and use that
    storedVideos[0].fileIndex = videos[0].fileIndex;
    storedVideos[0].title = videos[0].title;
    storedVideos[0].size = videos[0].size;
    storedVideos[0].subtitles = videos[0].subtitles;
    videos[0] = storedVideos[0];
  }
  // no videos available or more than one new videos were in the torrent
  const shouldDeleteOld = notOpenedVideo && videos.every(video => !video.id);

  return repository.createTorrent({ ...torrent, contents, subtitles })
      .then(() => {
        if (shouldDeleteOld) {
          logger.debug(`Deleting old video for [${torrent.infoHash}] ${torrent.title}`)
          return storedVideos[0].destroy();
        }
        return Promise.resolve();
      })
      .then(() => PromiseHelpers.sequence(videos.map(video => () => repository.createFile(video))))
      .then(() => logger.info(`Created contents for ${torrent.provider} [${torrent.infoHash}] ${torrent.title}`))
      .catch(error => logger.error(`Failed saving contents for [${torrent.infoHash}] ${torrent.title}`, error));
}

export async function updateTorrentSeeders(torrent) {
  if (!(torrent.infoHash || (torrent.provider && torrent.torrentId)) || !Number.isInteger(torrent.seeders)) {
    return torrent;
  }

  return repository.setTorrentSeeders(torrent, torrent.seeders)
      .catch(error => {
        logger.warn('Failed updating seeders:', error);
        return undefined;
      });
}
