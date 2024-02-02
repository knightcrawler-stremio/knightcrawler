import {extractSize} from "../jackett/jacketParser.js";

export default function sortStreams(streams, config) {
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
