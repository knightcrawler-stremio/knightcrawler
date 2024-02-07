import {IContentCreationAttributes} from "@repository/interfaces/content_attributes";
import {IFileAttributes, IFileCreationAttributes} from "@repository/interfaces/file_attributes";
import {ISubtitleAttributes, ISubtitleCreationAttributes} from "@repository/interfaces/subtitle_attributes";
import {ITorrentAttributes, ITorrentCreationAttributes} from "@repository/interfaces/torrent_attributes";
import {Content} from "@repository/models/content";
import {File} from "@repository/models/file";
import {Provider} from "@repository/models/provider";
import {SkipTorrent} from "@repository/models/skipTorrent";
import {Subtitle} from "@repository/models/subtitle";
import {Torrent} from "@repository/models/torrent";
import {WhereOptions} from "sequelize";
import {Model} from "sequelize-typescript";

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