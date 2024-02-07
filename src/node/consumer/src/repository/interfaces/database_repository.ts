import {Provider} from "../models/provider";
import {WhereOptions} from "sequelize";
import {ITorrentAttributes} from "./torrent_attributes";
import {Torrent} from "../models/torrent";
import {IFileAttributes} from "./file_attributes";
import {File} from "../models/file";
import {Subtitle} from "../models/subtitle";
import {Model} from "sequelize-typescript";
import {Content} from "../models/content";
import {SkipTorrent} from "../models/skipTorrent";

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

    createTorrent(torrent: Torrent): Promise<void>;

    setTorrentSeeders(torrent: ITorrentAttributes, seeders: number): Promise<[number]>;

    deleteTorrent(infoHash: string): Promise<number>;

    createFile(file: File): Promise<void>;

    getFiles(infoHash: string): Promise<File[]>;

    getFilesBasedOnTitle(titleQuery: string): Promise<File[]>;

    deleteFile(id: number): Promise<number>;

    createSubtitles(infoHash: string, subtitles: Subtitle[]): Promise<void | Model<any, any>[]>;

    upsertSubtitles(file: File, subtitles: Subtitle[]): Promise<void>;

    getSubtitles(infoHash: string): Promise<Subtitle[]>;

    getUnassignedSubtitles(): Promise<Subtitle[]>;

    createContents(infoHash: string, contents: Content[]): Promise<void>;

    getContents(infoHash: string): Promise<Content[]>;

    getSkipTorrent(infoHash: string): Promise<SkipTorrent>;

    createSkipTorrent(torrent: Torrent): Promise<[SkipTorrent, boolean]>;
}