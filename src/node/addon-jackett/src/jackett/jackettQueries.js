import {Type} from "../lib/types.js";

const moviesByYear = (cleanName, year) => `&cat=2000,2010,2020,2030,2040,2045,2050,2080&t=movie&q=${cleanName}&year=${year}`;
const movies = (cleanName) => `&cat=2000,2010,2020,2030,2040,2045,2050,2080&t=movie&q=${cleanName}`;
const seriesByEpisode = (cleanName, season, episode) => `&cat=5000,5010,5020,5030,5040,5045,5050,5060,5070,5080&t=tvsearch&q=${cleanName}&season=${season}&ep=${episode}`;

const getMovieSearchQueries = (cleanName, year) => {
    if (year) {
        return {
            moviesByYear: moviesByYear(cleanName, year),
        };
    }
    return {
        movies: movies(cleanName),
    };
}

const getSeriesSearchQueries = (cleanName, year, season, episode) => {
    return {
        seriesByEpisode: seriesByEpisode(cleanName, season, episode),
    };    
}

export const jackettSearchQueries = (cleanName, type, year, season, episode) => {
    switch (type) {
        case Type.MOVIE:
            return getMovieSearchQueries(cleanName, year);
        case Type.SERIES:
            return getSeriesSearchQueries(cleanName, year, season, episode);
            
        default:
            return { };
    }
};