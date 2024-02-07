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
import {ILoggingService} from "../lib/interfaces/logging_service";
import {IocTypes} from "../lib/models/ioc_types";
import {inject, injectable} from "inversify";
import {IDatabaseRepository} from "./interfaces/database_repository";

@injectable()
export class DatabaseRepository implements IDatabaseRepository {
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
    private logger: ILoggingService;
    
    constructor(@inject(IocTypes.ILoggingService) logger: ILoggingService) {
        this.logger = logger;
        this.database = this.createDatabase();
    }

    public connect = async () => {
        try {
            await this.database.sync({alter: configurationService.databaseConfig.AUTO_CREATE_AND_APPLY_MIGRATIONS});
        } catch {
            this.logger.error('Failed syncing database');
            process.exit(1);
        }
    };

    public getProvider = async (provider: Provider) => {
        try {
            const [result] = await Provider.findOrCreate({where: {name: {[Op.eq]: provider.name}}, defaults: provider});
            return result;
        } catch {
            return provider as Provider;
        }
    };

    public getTorrent = async (torrent: ITorrentAttributes): Promise<Torrent | null> => {
        const where = torrent.infoHash
            ? {infoHash: torrent.infoHash}
            : {provider: torrent.provider, torrentId: torrent.torrentId};
        return await Torrent.findOne({where});
    };

    public getTorrentsBasedOnTitle = async (titleQuery: string, type: string): Promise<Torrent[]> => this.getTorrentsBasedOnQuery({
        title: {[Op.regexp]: `${titleQuery}`},
        type
    });

    public getTorrentsBasedOnQuery = async (where: WhereOptions<ITorrentAttributes>): Promise<Torrent[]> => await Torrent.findAll({where});

    public getFilesBasedOnQuery = async (where: WhereOptions<IFileAttributes>): Promise<File[]> => await File.findAll({where});

    public getTorrentsWithoutSize = async (): Promise<Torrent[]> => await Torrent.findAll({
        where: literal(
            'exists (select 1 from files where files."infoHash" = torrent."infoHash" and files.size = 300000000)'),
        order: [
            ['seeders', 'DESC']
        ]
    });

    public getUpdateSeedersTorrents = async (limit = 50): Promise<Torrent[]> => {
        const until = moment().subtract(7, 'days').format('YYYY-MM-DD');
        return await Torrent.findAll({
            where: literal(`torrent."updatedAt" < '${until}'`),
            limit: limit,
            order: [
                ['seeders', 'DESC'],
                ['updatedAt', 'ASC']
            ]
        });
    };

    public getUpdateSeedersNewTorrents = async (limit = 50): Promise<Torrent[]> => {
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
    };

    public getNoContentsTorrents = async (): Promise<Torrent[]> => await Torrent.findAll({
        where: {opened: false, seeders: {[Op.gte]: 1}},
        limit: 500,
        order: literal('random()')
    });

    public createTorrent = async (torrent: Torrent): Promise<void> => {
        await Torrent.upsert(torrent);
        await this.createContents(torrent.infoHash, torrent.contents);
        await this.createSubtitles(torrent.infoHash, torrent.subtitles);
    };

    public setTorrentSeeders = async (torrent: ITorrentAttributes, seeders: number): Promise<[number]> => {
        const where = torrent.infoHash
            ? {infoHash: torrent.infoHash}
            : {provider: torrent.provider, torrentId: torrent.torrentId};

        return await Torrent.update(
            {seeders: seeders},
            {where: where}
        );
    };

    public deleteTorrent = async (infoHash: string): Promise<number> => await Torrent.destroy({where: {infoHash: infoHash}});

    public createFile = async (file: File): Promise<void> => {
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
            await File.create(file, {include: [Subtitle], ignoreDuplicates: true});
        }
    };

    public getFiles = async (infoHash: string): Promise<File[]> => File.findAll({where: {infoHash: infoHash}});

    public getFilesBasedOnTitle = async (titleQuery: string): Promise<File[]> => File.findAll({where: {title: {[Op.regexp]: `${titleQuery}`}}});

    public deleteFile = async (id: number): Promise<number> => File.destroy({where: {id: id}});

    public createSubtitles = async (infoHash: string, subtitles: Subtitle[]): Promise<void | Model<any, any>[]> => {
        if (subtitles && subtitles.length) {
            return Subtitle.bulkCreate(subtitles.map(subtitle => ({infoHash, title: subtitle.path, ...subtitle})));
        }
        return Promise.resolve();
    };

    public upsertSubtitles = async (file: File, subtitles: Subtitle[]): Promise<void> => {
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
    };

    public getSubtitles = async (infoHash: string): Promise<Subtitle[]> => Subtitle.findAll({where: {infoHash: infoHash}});

    public getUnassignedSubtitles = async (): Promise<Subtitle[]> => Subtitle.findAll({where: {fileId: null}});

    public createContents = async (infoHash: string, contents: Content[]): Promise<void> => {
        if (contents && contents.length) {
            await Content.bulkCreate(contents.map(content => ({infoHash, ...content})), {ignoreDuplicates: true});
            await Torrent.update({opened: true}, {where: {infoHash: infoHash}, silent: true});
        }
    };

    public getContents = async (infoHash: string): Promise<Content[]> => Content.findAll({where: {infoHash: infoHash}});

    public getSkipTorrent = async (infoHash: string): Promise<SkipTorrent> => {
        const result = await SkipTorrent.findByPk(infoHash);
        if (!result) {
            throw new Error(`torrent not found: ${infoHash}`);
        }
        return result.dataValues as SkipTorrent;
    };

    public createSkipTorrent = async (torrent: Torrent): Promise<[SkipTorrent, boolean]> => SkipTorrent.upsert({infoHash: torrent.infoHash});

    private createDatabase = (): Sequelize => {
        const newDatabase = new Sequelize(
            configurationService.databaseConfig.POSTGRES_URI,
            {
                logging: false
            }
        );
        
        newDatabase.addModels(this.models);
        
        return newDatabase;
    };
}
