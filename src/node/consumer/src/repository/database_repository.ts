import moment from 'moment';
import {literal, Op, WhereOptions} from "sequelize";
import {Model, Sequelize} from 'sequelize-typescript';
import {configurationService} from '../lib/services/configuration_service';
import {PromiseHelpers} from '../lib/helpers/promises_helpers';
import {Provider} from "./models/provider";
import {File} from "./models/file";
import {Torrent} from "./models/torrent";
import {IngestedTorrent} from "./models/ingestedTorrent";
import {Subtitle} from "./models/subtitle";
import {Content} from "./models/content";
import {SkipTorrent} from "./models/skipTorrent";
import {IFileAttributes} from "./interfaces/file_attributes";
import {ITorrentAttributes} from "./interfaces/torrent_attributes";
import {IngestedPage} from "./models/ingestedPage";
import {logger} from "../lib/services/logging_service";

class DatabaseRepository {
    private readonly database: Sequelize;
    
    private models = [
        Torrent,
        Provider,
        File,
        Subtitle,
        Content,
        SkipTorrent,
        IngestedTorrent,
        IngestedPage];
    
    constructor() {
        this.database = this.createDatabase();
    }
    
    public async connect() {
        try {
            await this.database.sync({alter: configurationService.databaseConfig.AUTO_CREATE_AND_APPLY_MIGRATIONS});
        } catch {
            logger.error('Failed syncing database');
            process.exit(1);
        }
    }

    public async getProvider(provider: Provider) {
        try {
            const [result] = await Provider.findOrCreate({ where: { name: { [Op.eq]: provider.name } }, defaults: provider });
            return result;
        } catch {
            return provider as Provider;
        }
    }

    public async getTorrent(torrent: ITorrentAttributes): Promise<Torrent | null> {
        const where = torrent.infoHash
            ? { infoHash: torrent.infoHash }
            : { provider: torrent.provider, torrentId: torrent.torrentId };
        return await Torrent.findOne({ where });
    }

    public async getTorrentsBasedOnTitle(titleQuery: string, type: string): Promise<Torrent[]> {
        return this.getTorrentsBasedOnQuery({ title: { [Op.regexp]: `${titleQuery}` }, type });
    }

    public async getTorrentsBasedOnQuery(where: WhereOptions<ITorrentAttributes>): Promise<Torrent[]> {
        return await Torrent.findAll({ where });
    }

    public async getFilesBasedOnQuery(where: WhereOptions<IFileAttributes>): Promise<File[]> {
        return await File.findAll({ where });
    }

    public async getTorrentsWithoutSize(): Promise<Torrent[]> {
        return await Torrent.findAll({
            where: literal(
                'exists (select 1 from files where files."infoHash" = torrent."infoHash" and files.size = 300000000)'),
            order: [
                ['seeders', 'DESC']
            ]
        });
    }

    public async getUpdateSeedersTorrents(limit = 50): Promise<Torrent[]> {
        const until = moment().subtract(7, 'days').format('YYYY-MM-DD');
        return await Torrent.findAll({
            where: literal(`torrent."updatedAt" < '${until}'`),
            limit: limit,
            order: [
                ['seeders', 'DESC'],
                ['updatedAt', 'ASC']
            ]
        });
    }

    public async getUpdateSeedersNewTorrents(limit = 50): Promise<Torrent[]> {
        const lastUpdate = moment().subtract(12, 'hours').format('YYYY-MM-DD');
        const createdAfter = moment().subtract(4, 'days').format('YYYY-MM-DD');
        return await Torrent.findAll({
            where: literal(`torrent."updatedAt" < '${lastUpdate}' AND torrent."createdAt" > '${createdAfter}'`),
            limit: limit,
            order: [
                ['seeders', 'ASC'],
                ['updatedAt', 'ASC']
            ]
        });
    }

    public async getNoContentsTorrents(): Promise<Torrent[]> {
        return await Torrent.findAll({
            where: { opened: false, seeders: { [Op.gte]: 1 } },
            limit: 500,
            order: literal('random()')
        });
    }

    public async createTorrent(torrent: Torrent): Promise<void> {
        await Torrent.upsert(torrent);
        await this.createContents(torrent.infoHash, torrent.contents);
        await this.createSubtitles(torrent.infoHash, torrent.subtitles);
    }

    public async setTorrentSeeders(torrent: ITorrentAttributes, seeders: number): Promise<[number]> {
        const where = torrent.infoHash
            ? { infoHash: torrent.infoHash }
            : { provider: torrent.provider, torrentId: torrent.torrentId };
        
        return await Torrent.update(
            { seeders: seeders },
            { where: where }
        );
    }

    public async deleteTorrent(infoHash: string): Promise<number> {
        return await Torrent.destroy({ where: { infoHash: infoHash } });
    }

    public async createFile(file: File): Promise<void> {
        if (file.id) {
            if (file.dataValues) {
                await file.save();
            } else {
                await File.upsert(file);
            }
            await this.upsertSubtitles(file, file.subtitles);
        } else {
            if (file.subtitles && file.subtitles.length) {
                file.subtitles = file.subtitles.map(subtitle => {
                    subtitle.title = subtitle.path;
                    return subtitle;
                });
            }
            await File.create(file, { include: [Subtitle], ignoreDuplicates: true });
        }
    }

    public async getFiles(infoHash: string): Promise<File[]> {
        return File.findAll({ where: { infoHash: infoHash } });
    }

    public async getFilesBasedOnTitle(titleQuery: string): Promise<File[]> {
        return File.findAll({ where: { title: { [Op.regexp]: `${titleQuery}` } } });
    }

    public async deleteFile(id: number): Promise<number> {
        return File.destroy({ where: { id: id } });
    }

    public async createSubtitles(infoHash: string, subtitles: Subtitle[]): Promise<void | Model<any, any>[]> {
        if (subtitles && subtitles.length) {
            return Subtitle.bulkCreate(subtitles.map(subtitle => ({ infoHash, title: subtitle.path, ...subtitle })));
        }
        return Promise.resolve();
    }

    public async upsertSubtitles(file: File, subtitles: Subtitle[]): Promise<void> {
        if (file.id && subtitles && subtitles.length) {
            await PromiseHelpers.sequence(subtitles
                .map(subtitle => {
                    subtitle.fileId = file.id;
                    subtitle.infoHash = subtitle.infoHash || file.infoHash;
                    subtitle.title = subtitle.title || subtitle.path;
                    return subtitle;
                })
                .map(subtitle => async () => {
                    if (subtitle.dataValues) {
                        await subtitle.save();
                    } else {
                        await Subtitle.create(subtitle);
                    }
                }));
        }
    }

    public async getSubtitles(infoHash: string): Promise<Subtitle[]> {
        return Subtitle.findAll({ where: { infoHash: infoHash } });
    }

    public async getUnassignedSubtitles(): Promise<Subtitle[]> {
        return Subtitle.findAll({ where: { fileId: null } });
    }

    public async createContents(infoHash: string, contents: Content[]): Promise<void> {
        if (contents && contents.length) {
            await Content.bulkCreate(contents.map(content => ({ infoHash, ...content })), { ignoreDuplicates: true });
            await Torrent.update({ opened: true }, { where: { infoHash: infoHash }, silent: true });
        }
    }

    public async getContents(infoHash: string): Promise<Content[]> {
        return Content.findAll({ where: { infoHash: infoHash } });
    }

    public async getSkipTorrent(infoHash: string): Promise<SkipTorrent> {
        const result = await SkipTorrent.findByPk(infoHash);
        if (!result) {
            throw new Error(`torrent not found: ${infoHash}`);
        }
        return result.dataValues as SkipTorrent;
    }

    public async createSkipTorrent(torrent: Torrent): Promise<[SkipTorrent, boolean]> {
        return SkipTorrent.upsert({ infoHash: torrent.infoHash });
    }
    
    private createDatabase(): Sequelize {
        const newDatabase = new Sequelize(
            configurationService.databaseConfig.POSTGRES_URI,
            {
                logging: false
            }
        );
        
        newDatabase.addModels(this.models);
        
        return newDatabase;
    }
}

export const repository = new DatabaseRepository();
