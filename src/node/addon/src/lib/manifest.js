import { MochOptions } from '../moch/moch.js';
import { showDebridCatalog } from '../moch/options.js';
import { Type } from './types.js';

const CatalogMochs = Object.values(MochOptions).filter(moch => moch.catalog);

export function manifest(config = {}) {
  return {
    id: 'com.stremio.knightcrawler.knightcrawler',
    version: 'v0.0.1',
    name: getName(config),
    description: getDescription(),
    catalogs: getCatalogs(config),
    resources: getResources(config),
    types: [Type.MOVIE, Type.SERIES, Type.ANIME, Type.OTHER],
    background: "https://i.ibb.co/9pXGycn/logo-color.png",
    logo: "https://i.ibb.co/hYJPLdP/logo-only.png",
    behaviorHints: {
      configurable: true,
      configurationRequired: false,
    }
  };
}

export function dummyManifest() {
  const manifestDefault = manifest();
  manifestDefault.catalogs = [{ id: 'dummy', type: Type.OTHER }];
  manifestDefault.resources = ['stream', 'meta'];
  return manifestDefault;
}

function getName(config) {
  const rootName = 'Knight Crawler';
  const mochSuffix = Object.values(MochOptions)
      .filter(moch => config[moch.key])
      .map(moch => moch.shortName)
      .join('/');
  return [rootName, mochSuffix].filter(v => v).join(' ');
}

function getDescription() {
  return 'Selfhost the Torrentio brings you much Funio';
}

function getCatalogs(config) {
  return CatalogMochs
      .filter(moch => showDebridCatalog(config) && config[moch.key])
      .map(moch => ({
        id: `knightcrawler-${moch.key}`,
        name: `${moch.name}`,
        type: 'other',
        extra: [{ name: 'skip' }],
      }));
}

function getResources(config) {
  const streamResource = {
    name: 'stream',
    types: [Type.MOVIE, Type.SERIES],
    idPrefixes: ['tt', 'kitsu']
  };
  const metaResource = {
    name: 'meta',
    types: [Type.OTHER],
    idPrefixes: CatalogMochs.filter(moch => config[moch.key]).map(moch => moch.key)
  };
  if (showDebridCatalog(config) && CatalogMochs.filter(moch => config[moch.key]).length) {
    return [streamResource, metaResource];
  }
  return [streamResource];
}
