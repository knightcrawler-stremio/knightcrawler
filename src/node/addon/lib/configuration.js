import { DebridOptions } from '../moch/options.js';
import { QualityFilter, Providers, SizeFilter } from './filter.js';
import { LanguageOptions } from './languages.js';

const keysToSplit = [Providers.key, LanguageOptions.key, QualityFilter.key, SizeFilter.key, DebridOptions.key];
const keysToUppercase = [SizeFilter.key];

export function parseConfiguration(configuration) {
  if (!configuration) {
    return undefined;
  }
  
  const configValues = configuration.split('|')
      .reduce((map, next) => {
        const parameterParts = next.split('=');
        if (parameterParts.length === 2) {
          map[parameterParts[0].toLowerCase()] = parameterParts[1];
        }
        return map;
      }, {});
  keysToSplit
      .filter(key => configValues[key])
      .forEach(key => configValues[key] = configValues[key].split(',')
          .map(value => keysToUppercase.includes(key) ? value.toUpperCase() : value.toLowerCase()))
  return configValues;
}

function configValue(config) {
  return Object.entries(config)
      .map(([key, value]) => `${key}=${Array.isArray(value) ? value.join(',') : value}`)
      .join('|');
}