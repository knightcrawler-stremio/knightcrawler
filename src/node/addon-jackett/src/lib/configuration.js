import { DebridOptions } from '../moch/options.js';

const keysToSplit = [DebridOptions.key];

export function parseConfiguration(configuration) {
  if (!configuration) {
    return undefined;
  }

    const configValues = configuration.split('|')
        .reduce((map, next) => {
            const [key, value] = next.split('=');
            if (key && value) {
                map[key.toLowerCase()] = value;
            }
            return map;
        }, {});

    keysToSplit
        .filter(key => configValues[key])
        .forEach(key => configValues[key] = configValues[key].split(',')
            .map(value => keysToUppercase.includes(key) ? value.toUpperCase() : value.toLowerCase()))

    return configValues;
}