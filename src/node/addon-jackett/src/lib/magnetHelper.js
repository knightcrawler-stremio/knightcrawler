import axios from 'axios';
import magnet from 'magnet-uri';
import { getRandomUserAgent } from './requestHelper.js';

const TRACKERS_URL = 'https://raw.githubusercontent.com/ngosang/trackerslist/master/trackers_best.txt';
const DEFAULT_TRACKERS = [
  "udp://47.ip-51-68-199.eu:6969/announce",
  "udp://9.rarbg.me:2940",
  "udp://9.rarbg.to:2820",
  "udp://exodus.desync.com:6969/announce",
  "udp://explodie.org:6969/announce",
  "udp://ipv4.tracker.harry.lu:80/announce",
  "udp://open.stealth.si:80/announce",
  "udp://opentor.org:2710/announce",
  "udp://opentracker.i2p.rocks:6969/announce",
  "udp://retracker.lanta-net.ru:2710/announce",
  "udp://tracker.cyberia.is:6969/announce",
  "udp://tracker.dler.org:6969/announce",
  "udp://tracker.ds.is:6969/announce",
  "udp://tracker.internetwarriors.net:1337",
  "udp://tracker.openbittorrent.com:6969/announce",
  "udp://tracker.opentrackr.org:1337/announce",
  "udp://tracker.tiny-vps.com:6969/announce",
  "udp://tracker.torrent.eu.org:451/announce",
  "udp://valakas.rollo.dnsabr.com:2710/announce",
  "udp://www.torrent.eu.org:451/announce",
]

let BEST_TRACKERS = [];

export async function getMagnetLink(infohash) {
  const trackers = [].concat(DEFAULT_TRACKERS).concat(BEST_TRACKERS);
  return magnet.encode({ infohash: infohash, announce: trackers });
}

export async function initBestTrackers() {
  BEST_TRACKERS = await getBestTrackers();
}

async function getBestTrackers(retry = 2) {
  const options = { timeout: 30000, headers: { 'User-Agent': getRandomUserAgent() } };
  return axios.get(TRACKERS_URL, options)
      .then(response => response?.data?.trim()?.split('\n\n') || [])
      .catch(error => {
        if (retry === 0) {
          console.log(`Failed retrieving best trackers: ${error.message}`);
          throw error;
        }
        return getBestTrackers(retry - 1);
      });
}

export function getSources(magnetInfo) {
  if (!magnetInfo.announce) {
    return null;
  }
  const trackers = Array.isArray(magnetInfo.announce) ? magnetInfo.announce : magnetInfo.announce.split(',');
  return trackers.map(tracker => `tracker:${tracker}`).concat(`dht:${magnetInfo.infohash}`);
}