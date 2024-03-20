const METAHUB_URL = 'https://images.metahub.space'
export const BadTokenError = { code: 'BAD_TOKEN' }
export const AccessDeniedError = { code: 'ACCESS_DENIED' }

export function chunkArray(arr, size) {
  return arr.length > size
      ? [arr.slice(0, size), ...chunkArray(arr.slice(size), size)]
      : [arr];
}

export function streamFilename(stream) {
  const titleParts = stream.title.replace(/\n👤.*/s, '').split('\n');
  const filename = titleParts.pop().split('/').pop();
  return encodeURIComponent(filename)
}

export async function enrichMeta(imdbId) {
  return {
    id: imdbId,
    thumbnail: `${METAHUB_URL}/background/small/${imdbId}/img`
  };
}

export function sameFilename(filename, expectedFilename) {
  const offset = filename.length - expectedFilename.length;
  for (let i = 0; i < expectedFilename.length; i++) {
    if (filename[offset + i] !== expectedFilename[i] && expectedFilename[i] !== '�') {
      return false;
    }
  }
  return true;
}

function mostCommonValue(array) {
  return array.sort((a, b) => array.filter(v => v === a).length - array.filter(v => v === b).length).pop();
}
