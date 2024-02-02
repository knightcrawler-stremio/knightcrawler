import { containsLanguage, LanguageOptions } from './languages.js';
import { extractSize } from './titleHelper.js';

export const SortOptions = {
  key: 'sort',
  options: {
    qualitySeeders: {
      key: 'quality',
      description: 'By quality then seeders'
    },
    qualitySize: {
      key: 'qualitysize',
      description: 'By quality then size'
    },
    seeders: {
      key: 'seeders',
      description: 'By seeders'
    },
    size: {
      key: 'size',
      description: 'By size'
    },
  }
}

export default function sortStreams(streams, config) {
  const languages = config[LanguageOptions.key];
  if (languages?.length && languages[0] !== 'english') {
    // No need to filter english since it's hard to predict which entries are english
    const streamsWithLanguage = streams.filter(stream => containsLanguage(stream, languages));
    const streamsNoLanguage = streams.filter(stream => !streamsWithLanguage.includes(stream));
    return _sortStreams(streamsWithLanguage, config).concat(_sortStreams(streamsNoLanguage, config));
  }
  return _sortStreams(streams, config);
}

function _sortStreams(streams, config) {
  const limit = /^[1-9][0-9]*$/.test(config.limit) && parseInt(config.limit) || undefined;
    
  return sortBySize(streams, limit);
}

function sortBySize(streams, limit) {
  return streams
      .sort((a, b) => {
        const aSize = extractSize(a.title);
        const bSize = extractSize(b.title);
        return bSize - aSize;
      }).slice(0, limit);
}
