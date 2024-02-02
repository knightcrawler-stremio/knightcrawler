import {decode} from "magnet-uri";
import titleParser from 'parse-torrent-title';
import {getSources} from './magnetHelper.js';
import { Type } from './types.js';

const ADDON_NAME = 'jackettio';
const UNKNOWN_SIZE = 300000000;

export function toStreamInfo(record, type) {
  const torrentInfo = titleParser.parse(record.title);
  const fileInfo = titleParser.parse(record.title);
  const title = joinDetailParts(
      [
        joinDetailParts([record.title.replace(/[, ]+/g, ' ')]),
        joinDetailParts([
          joinDetailParts([formatSize(record.size)], '💾 ')
        ]),
      ],
      '',
      '\n'
  );
  const name = joinDetailParts(
      [
        joinDetailParts([ADDON_NAME]),
      ],
      '',
      '\n'
  );
  const bingeGroupParts = getBingeGroupParts(record, torrentInfo, fileInfo, type);
  const bingeGroup = joinDetailParts(bingeGroupParts, "jackettio|", "|")
  const behaviorHints = bingeGroup ? { bingeGroup } : undefined;

  const magnetInfo = decode(record.magneturl)
  
  return cleanOutputObject({
    name: name,
    title: title,
    infohash: record.infohash,
    behaviorHints: behaviorHints,
    sources: getSources(magnetInfo),
  });
}

function joinDetailParts(parts, prefix = '', delimiter = ' ') {
  const filtered = parts.filter((part) => part !== undefined && part !== null).join(delimiter);

  return filtered.length > 0 ? `${prefix}${filtered}` : undefined;
}

function formatSize(size) {
  if (!size) {
    return undefined;
  }
  if (size === UNKNOWN_SIZE) {
    return undefined;
  }
  const i = size === 0 ? 0 : Math.floor(Math.log(size) / Math.log(1024));
  return Number((size / Math.pow(1024, i)).toFixed(2)) + ' ' + ['B', 'kB', 'MB', 'GB', 'TB'][i];
}

function getBingeGroupParts(record, sameInfo, quality, torrentInfo, fileInfo, type) {
  if (type === Type.MOVIE) {
    return [quality];
  
  } else if (sameInfo) {
    return [quality];
  }
  return [record.infohash];
}

function cleanOutputObject(object) {
  return Object.fromEntries(Object.entries(object).filter(([_, v]) => v != null));
}
