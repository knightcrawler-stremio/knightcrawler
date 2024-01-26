const axios = require('axios');
const cheerio = require('cheerio');
const Sugar = require('sugar-date');
const decode = require('magnet-uri');
const Promises = require('../../lib/promises');
const { escapeHTML } = require('../../lib/metadata');
const { getRandomUserAgent } = require('../../lib/requestHelper');
const { parseSize } = require("../scraperHelper");

const defaultProxies = [
  'https://1337x.to'
];
const defaultTimeout = 10000;
const maxSearchPage = 50;

const Categories = {
  MOVIE: 'Movies',
  TV: 'TV',
  ANIME: 'Anime',
  DOCUMENTARIES: 'Documentaries',
  APPS: 'Apps',
  GAMES: 'Games',
  MUSIC: 'Music',
  PORN: 'XXX',
  OTHER: 'Other',
};

function torrent(torrentId, config = {}, retries = 2) {
  if (!torrentId || retries === 0) {
    return Promise.reject(new Error(`Failed ${torrentId} query`));
  }
  const proxyList = config.proxyList || defaultProxies;
  const slug = torrentId.startsWith('/torrent/') ? torrentId.replace('/torrent/', '') : torrentId;

  return Promises.first(proxyList
          .map((proxyUrl) => singleRequest(`${proxyUrl}/torrent/${slug}`, config)))
      .then((body) => parseTorrentPage(body))
      .then((torrent) => ({ torrentId: slug, ...torrent }))
      .catch((err) => torrent(slug, config, retries - 1));
}

function search(keyword, config = {}, retries = 2) {
  if (!keyword || retries === 0) {
    return Promise.reject(new Error(`Failed ${keyword} search`));
  }
  const proxyList = config.proxyList || defaultProxies;
  const page = config.page || 1;
  const category = config.category;
  const extendToPage = Math.min(maxSearchPage, (config.extendToPage || 1))
  const requestUrl = proxyUrl => category
      ? `${proxyUrl}/category-search/${keyword}/${category}/${page}/`
      : `${proxyUrl}/search/${keyword}/${page}/`;

  return Promises.first(proxyList
          .map(proxyUrl => singleRequest(requestUrl(proxyUrl), config)))
      .then(body => parseTableBody(body))
      .then(torrents => torrents.length === 40 && page < extendToPage
          ? search(keyword, { ...config, page: page + 1 }).catch(() => [])
              .then(nextTorrents => torrents.concat(nextTorrents))
          : torrents)
      .catch((err) => search(keyword, config, retries - 1));
}

function browse(config = {}, retries = 2) {
  if (retries === 0) {
    return Promise.reject(new Error(`Failed browse request`));
  }
  const proxyList = config.proxyList || defaultProxies;
  const page = config.page || 1;
  const category = config.category;
  const sort = config.sort;
  const requestUrl = proxyUrl => sort
      ? `${proxyUrl}/sort-cat/${category}/${sort}/desc/${page}/`
      : `${proxyUrl}/cat/${category}/${page}/`;

  return Promises.first(proxyList
          .map((proxyUrl) => singleRequest(requestUrl(proxyUrl), config)))
      .then((body) => parseTableBody(body))
      .catch((err) => browse(config, retries - 1));
}

const singleRequest = (requestUrl, config = {}) => {
  const defaultTimeout = 60000; // Define the default timeout if it's not provided in the config
  const timeout = config.timeout || defaultTimeout;

  const payload = {
    cmd: "request.get",
    url: requestUrl,
    maxTimeout: timeout
  };

  const headers = {
    'Content-Type': 'application/json',
    // Include other headers if necessary
  };

  const options = {
    headers: headers,
    timeout: timeout,
  };

  return axios.post(process.env.FLARESOLVERR_ENDPOINT, payload, options)
    .then((response) => {
      const body = response.data;
      if (!body || !body.solution || !body.solution.response) {
        throw new Error(`Invalid response structure: ${JSON.stringify(body)}`);
      }
      
      const solutionResponse = body.solution.response;

      // Here you can further process solutionResponse or return it
      // For instance, check if it contains '502: Bad gateway', '403 Forbidden', or '1337x</title>'
      if (solutionResponse.includes('502: Bad gateway') ||
          solutionResponse.includes('403 Forbidden') ||
          !(solutionResponse.includes('1337x</title>'))) {
        throw new Error(`Invalid body contents: ${requestUrl}`);
      }

      return solutionResponse;
    });
};


function parseTableBody(body) {
  return new Promise((resolve, reject) => {
    const $ = cheerio.load(body);

    if (!$) {
      reject(new Error('Failed loading body'));
    }

    const torrents = [];

    $('.table > tbody > tr').each((i, element) => {
      const row = $(element);
      torrents.push({
        name: row.find('a').eq(1).text(),
        torrentId: row.find('a').eq(1).attr('href').replace('/torrent/', ''),
        seeders: parseInt(row.children('td.coll-2').text()),
        leechers: parseInt(row.children('td.coll-3').text()),
        size: parseSize(row.children('td.coll-4').text())
      });
    });

    resolve(torrents);
  });
}

function parseTorrentPage(body) {
  return new Promise((resolve, reject) => {
    const $ = cheerio.load(body);

    if (!$) {
      reject(new Error('Failed loading body'));
    }

    const details = $('.torrent-detail-page');
    const magnetLink = details.find('a:contains(\'Magnet Download\')').attr('href');
    const imdbIdMatch = details.find('div[id=\'description\']').html().match(/imdb\.com\/title\/(tt\d+)/i);

    const torrent = {
      name: escapeHTML(decode(magnetLink).name.replace(/\+/g, ' ')),
      infoHash: decode(magnetLink).infoHash,
      magnetLink: magnetLink,
      seeders: parseInt(details.find('strong:contains(\'Seeders\')').next().text(), 10),
      leechers: parseInt(details.find('strong:contains(\'Leechers\')').next().text(), 10),
      category: details.find('strong:contains(\'Category\')').next().text(),
      languages: details.find('strong:contains(\'Language\')').next().text(),
      size: parseSize(details.find('strong:contains(\'Total size\')').next().text()),
      uploadDate: parseDate(details.find('strong:contains(\'Date uploaded\')').next().text()),
      imdbId: imdbIdMatch && imdbIdMatch[1],
      files: details.find('div[id=\'files\']').first().find('li')
          .map((i, elem) => $(elem).text())
          .map((i, text) => ({
            fileIndex: i,
            name: text.match(/^(.+)\s\(.+\)$/)[1].replace(/^.+\//g, ''),
            path: text.match(/^(.+)\s\(.+\)$/)[1],
            size: parseSize(text.match(/^.+\s\((.+)\)$/)[1])
          })).get()
    };
    resolve(torrent);
  });
}

function parseDate(dateString) {
  if (/decade.*ago/i.test(dateString)) {
    return Sugar.Date.create('10 years ago');
  }
  return Sugar.Date.create(dateString);
}

module.exports = { torrent, search, browse, Categories };
