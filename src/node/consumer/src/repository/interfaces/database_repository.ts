import {Provider} from "../models/provider";
import {WhereOptions} from "sequelize";
import {ITorrentAttributes, ITorrentCreationAttributes} from "./torrent_attributes";
import {Torrent} from "../models/torrent";
import {IFileAttributes, IFileCreationAttributes} from "./file_attributes";
import {File} from "../models/file";
import {Subtitle} from "../models/subtitle";
import {Model} from "sequelize-typescript";
import {Content} from "../models/content";
import {SkipTorrent} from "../models/skipTorrent";
import {ISubtitleCreationAttributes} from "./subtitle_attributes";
import {IContentCreationAttributes} from "./content_attributes";

export interface IDatabaseRepository {
    connect(): Promise<void>;

    getProvider(provider: Provider): Promise<Provider>;

    getTorrent(torrent: ITorrentAttributes): Promise<Torrent | null>;

    getTorrentsBasedOnTitle(titleQuery: string, type: string): Promise<Torrent[]>;

    getTorrentsBasedOnQuery(where: WhereOptions<ITorrentAttributes>): Promise<Torrent[]>;

    getFilesBasedOnQuery(where: WhereOptions<IFileAttributes>): Promise<File[]>;

    getTorrentsWithoutSize(): Promise<Torrent[]>;

    getUpdateSeedersTorrents(limit): Promise<Torrent[]>;

    getUpdateSeedersNewTorrents(limit): Promise<Torrent[]>;

    getNoContentsTorrents(): Promise<Torrent[]>;

    createTorrent(torrent: ITorrentCreationAttributes): Promise<void>;

    setTorrentSeeders(torrent: ITorrentAttributes, seeders: number): Promise<[number]>;

    deleteTorrent(infoHash: string): Promise<number>;

    createFile(file: IFileCreationAttributes): Promise<void>;

    getFiles(infoHash: string): Promise<File[]>;

    getFilesBasedOnTitle(titleQuery: string): Promise<File[]>;

    deleteFile(id: number): Promise<number>;

    createSubtitles(infoHash: string, subtitles: ISubtitleCreationAttributes[]): Promise<void | Model<any, any>[]>;

    upsertSubtitles(file: File, subtitles: Subtitle[]): Promise<void>;

    getSubtitles(infoHash: string): Promise<Subtitle[]>;

    getUnassignedSubtitles(): Promise<Subtitle[]>;

    createContents(infoHash: string, contents: IContentCreationAttributes[]): Promise<void>;

    getContents(infoHash: string): Promise<Content[]>;

    getSkipTorrent(infoHash: string): Promise<SkipTorrent>;

    createSkipTorrent(torrent: ITorrentCreationAttributes): Promise<[SkipTorrent, boolean]>;
}