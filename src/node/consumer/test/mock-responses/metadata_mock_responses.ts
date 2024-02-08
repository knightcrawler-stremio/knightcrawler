import {http, HttpResponse} from "msw";
import cinemetaQuery from "./assets/cinemeta-query-response.json";
import cinemetaFlashFull from "./assets/flash-episode-list.json";
import kitsuNarutoFull from "./assets/kitsu-naruto-full.json";
import imdbTheFlash from "./assets/name-to-imdb-flash.json";
import kitsuNarutoSearchId from "./assets/test-kitsu-search-id-naruto.json";

const kitsuNarutoIdSearchTestResponse = http.get('https://anime-kitsu.strem.fun/catalog/series/kitsu-anime-list/search=naruto%202002%20S1.json', () => {
    return HttpResponse.json(kitsuNarutoSearchId);
});

const kitsuNarutoMetaDataSearchTestResponse = http.get('https://anime-kitsu.strem.fun/meta/Series/kitsu:11.json', () => {
    return HttpResponse.json(kitsuNarutoFull);
});

const nameToImdbTheFlash = http.get('https://sg.media-imdb.com/suggests/t/the%20flash%201990.json', () => {
    const jsonpResponse = `/**/imdb$the_flash%201990(${JSON.stringify(imdbTheFlash)});`;
    return HttpResponse.json(jsonpResponse);
});

const cinemetaQueryResponse = http.get('https://cinemeta.strem.io/stremioget/stremio/v1/q.json', () => {
    return HttpResponse.json(cinemetaQuery);
});

const cinemetaFlashMetadataSearchTestResponse = http.get('https://v3-cinemeta.strem.io/meta/imdb/tt0098798.json', () => {
    return HttpResponse.json(cinemetaFlashFull);
});

const checkIfImdbEpisode = http.get('https://www.imdb.com/title/tt0579968/', () => {
    return HttpResponse.text('<meta property="og:type" content="video.episode">');
});

export {
    kitsuNarutoIdSearchTestResponse,
    kitsuNarutoMetaDataSearchTestResponse,
    nameToImdbTheFlash,
    cinemetaQueryResponse,
    cinemetaFlashMetadataSearchTestResponse,
    checkIfImdbEpisode
}