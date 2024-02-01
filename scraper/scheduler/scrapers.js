
const ytsScraper = require('../scrapers/yts/yts_scraper');
const ytsFullScraper = require('../scrapers/yts/yts_full_scraper');
const eztvScraper = require('../scrapers/eztv/eztv_scraper');
const leetxScraper = require('../scrapers/1337x/1337x_scraper');
const torrentGalaxyScraper = require('../scrapers/torrentgalaxy/torrentgalaxy_scraper');

module.exports = [
  { scraper: ytsScraper, name: ytsScraper.NAME, cron: '0 0 */4 ? * *' },
  { scraper: eztvScraper, name: eztvScraper.NAME, cron: '0 0 */4 ? * *' },
  { scraper: torrentGalaxyScraper, name: torrentGalaxyScraper.NAME, cron: '0 0 */4 ? * *' },
  { scraper: leetxScraper, name: leetxScraper.NAME, cron: '0 0 */4 ? * *' },
  { scraper: ytsFullScraper, name: ytsFullScraper.NAME, cron: '0 0 0 * * 0' }
];
