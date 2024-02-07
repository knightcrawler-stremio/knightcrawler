import {ICommonVideoMetadata} from "./common_video_metadata";

export interface ISeasonEpisodeMap {
    [season: number]: {
        [episode: number]: ICommonVideoMetadata;
    }
}