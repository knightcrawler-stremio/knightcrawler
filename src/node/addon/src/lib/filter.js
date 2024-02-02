import { parseSize, extractSize } from './titleHelper.js';
import { Type } from './types.js';
export const QualityFilter = {
  key: 'qualityfilter',
  options: [
    {
      key: 'brremux',
      label: 'BluRay REMUX',
      test(quality, bingeGroup) {
        return bingeGroup?.includes(this.label);
      }
    },
    {
      key: 'hdrall',
      label: 'HDR/HDR10+/Dolby Vision',
      items: ['HDR', 'HDR10+', 'DV'],
      test(quality) {
        const hdrProfiles = quality?.split(' ')?.slice(1)?.join() || '';
        return this.items.some(hdrType => hdrProfiles.includes(hdrType));
      }
    },
    {
      key: 'dolbyvision',
      label: 'Dolby Vision',
      test(quality) {
        const hdrProfiles = quality?.split(' ')?.slice(1)?.join() || '';
        return hdrProfiles === 'DV';
      }
    },
    {
      key: '4k',
      label: '4k',
      items: ['4k'],
      test(quality) {
        return quality && this.items.includes(quality.split(' ')[0]);
      }
    },
    {
      key: '1080p',
      label: '1080p',
      items: ['1080p'],
      test(quality) {
        return this.items.includes(quality)
      }
    },
    {
      key: '720p',
      label: '720p',
      items: ['720p'],
      test(quality) {
        return this.items.includes(quality)
      }
    },
    {
      key: '480p',
      label: '480p',
      items: ['480p'],
      test(quality) {
        return this.items.includes(quality)
      }
    },
    {
      key: 'other',
      label: 'Other (DVDRip/HDRip/BDRip...)',
      // could be ['DVDRip', 'HDRip', 'BDRip', 'BRRip', 'BluRay', 'WEB-DL', 'WEBRip', 'HDTV', 'DivX', 'XviD']
      items: ['4k', '1080p', '720p', '480p', 'SCR', 'CAM', 'TeleSync', 'TeleCine'],
      test(quality) {
        return quality && !this.items.includes(quality.split(' ')[0]);
      }
    },
    {
      key: 'scr',
      label: 'Screener',
      items: ['SCR'],
      test(quality) {
        return this.items.includes(quality)
      }
    },
    {
      key: 'cam',
      label: 'Cam',
      items: ['CAM', 'TeleSync', 'TeleCine'],
      test(quality) {
        return this.items.includes(quality)
      }
    },
    {
      key: 'unknown',
      label: 'Unknown',
      test(quality) {
        return !quality
      }
    }
  ]
};
export const SizeFilter = {
  key: 'sizefilter'
}

export default function applyFilters(streams, config) {
  return [
    filterByQuality,
    filterBySize
  ].reduce((filteredStreams, filter) => filter(filteredStreams, config), streams);
}

function filterByQuality(streams, config) {
  const filters = config[QualityFilter.key];
  if (!filters) {
    return streams;
  }
  const filterOptions = QualityFilter.options.filter(option => filters.includes(option.key));
  return streams.filter(stream => {
    const [ , streamQuality] = stream.name.split('\n');
    const bingeGroup = stream.behaviorHints?.bingeGroup;
    return !filterOptions.some(option => option.test(streamQuality, bingeGroup));
  });
}

function filterBySize(streams, config) {
  const sizeFilters = config[SizeFilter.key];
  if (!sizeFilters?.length) {
    return streams;
  }
  const sizeLimit = parseSize(config.type === Type.MOVIE ? sizeFilters.shift() : sizeFilters.pop());
  return streams.filter(stream => {
    const size = extractSize(stream.title)
    return size <= sizeLimit;
  })
}
