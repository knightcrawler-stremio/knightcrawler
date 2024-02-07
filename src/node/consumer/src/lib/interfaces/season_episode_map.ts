import {ICommonVideoMetadata} from "@interfaces/common_video_metadata";

export interface ISeasonEpisodeMap {
    [season: number]: {
        [episode: number]: ICommonVideoMetadata;
    }
}