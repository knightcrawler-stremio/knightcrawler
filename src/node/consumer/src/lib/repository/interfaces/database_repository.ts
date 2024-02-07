import {WhereOptions} from "sequelize";
import {Model} from "sequelize-typescript";
import {Content} from "../models/content";
import {File} from "../models/file";
import {Provider} from "../models/provider";
import {SkipTorrent} from "../models/skipTorrent";
import {Subtitle} from "../models/subtitle";
import {Torrent} from "../models/torrent";
import {IContentCreationAttributes} from "./content_attributes";
import {IFileAttributes, IFileCreationAttributes} from "./file_attributes";
import {ISubtitleAttributes, ISubtitleCreationAttributes} from "./subtitle_attributes";
import {ITorrentAttributes, ITorrentCreationAttributes} from "./torrent_attributes";

export interface IDatabaseRepository {
    connect(): Promise<void>;

    getProvider(provider: Provider): Promise<Provider>;

    getTorrent(torrent: ITorrentAttributes): Promise<Torrent | null>;

    getTorrentsBasedOnTitle(titleQuery: string, type: string): Promise<Torrent[]>;

    getTorrentsBasedOnQuery(where: WhereOptions<ITorrentAttributes>): Promise<Torrent[]>;

    getFilesBasedOnQuery(where: WhereOptions<IFileAttributes>): Promise<File[]>;

    getTorrentsWithoutSize(): Promise<Torrent[]>;

    getUpdateSeedersTorrents(limit: number): Promise<Torrent[]>;

    getUpdateSeedersNewTorrents(limit: number): Promise<Torrent[]>;

    getNoContentsTorrents(): Promise<Torrent[]>;

    createTorrent(torrent: ITorrentCreationAttributes): Promise<void>;

    setTorrentSeeders(torrent: ITorrentAttributes, seeders: number): Promise<[number]>;

    deleteTorrent(infoHash: string): Promise<number>;

    createFile(file: IFileCreationAttributes): Promise<void>;

    getFiles(infoHash: string): Promise<File[]>;

    getFilesBasedOnTitle(titleQuery: string): Promise<File[]>;

    deleteFile(id: number): Promise<number>;

    createSubtitles(infoHash: string, subtitles: ISubtitleCreationAttributes[]): Promise<void | Model<ISubtitleAttributes, ISubtitleCreationAttributes>[]>;

    upsertSubtitles(file: File, subtitles: ISubtitleCreationAttributes[] | undefined): Promise<void>;

    getSubtitles(infoHash: string): Promise<Subtitle[]>;

    getUnassignedSubtitles(): Promise<Subtitle[]>;

    createContents(infoHash: string, contents: IContentCreationAttributes[]): Promise<void>;

    getContents(infoHash: string): Promise<Content[]>;

    getSkipTorrent(infoHash: string): Promise<SkipTorrent>;

    createSkipTorrent(torrent: ITorrentCreationAttributes): Promise<[SkipTorrent, boolean | null]>;
}