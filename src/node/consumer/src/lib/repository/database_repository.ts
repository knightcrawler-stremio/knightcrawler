import {PromiseHelpers} from '@helpers/promises_helpers';
import {ILoggingService} from "@interfaces/logging_service";
import {IocTypes} from "@models/ioc_types";
import {IContentCreationAttributes} from "@repository/interfaces/content_attributes";
import {IDatabaseRepository} from "@repository/interfaces/database_repository";
import {IFileAttributes, IFileCreationAttributes} from "@repository/interfaces/file_attributes";
import {ISubtitleAttributes, ISubtitleCreationAttributes} from "@repository/interfaces/subtitle_attributes";
import {ITorrentAttributes, ITorrentCreationAttributes} from "@repository/interfaces/torrent_attributes";
import {Content} from "@repository/models/content";
import {File} from "@repository/models/file";
import {IngestedPage} from "@repository/models/ingestedPage";
import {IngestedTorrent} from "@repository/models/ingestedTorrent";
import {Provider} from "@repository/models/provider";
import {SkipTorrent} from "@repository/models/skipTorrent";
import {Subtitle} from "@repository/models/subtitle";
import {Torrent} from "@repository/models/torrent";
import {configurationService} from '@services/configuration_service';
import {inject, injectable} from "inversify";
import moment from 'moment';
import {literal, Op, WhereOptions} from "sequelize";
import {Model, Sequelize} from 'sequelize-typescript';

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

    public connect = async (): Promise<void> => {
        try {
            await this.database.sync({alter: configurationService.databaseConfig.AUTO_CREATE_AND_APPLY_MIGRATIONS});
        } catch (error) {
            this.logger.debug('Failed to sync database', error);
            this.logger.error('Failed syncing database');
            process.exit(1);
        }
    };

    public getProvider = async (provider: Provider): Promise<Provider> => {
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

    public createTorrent = async (torrent: ITorrentCreationAttributes): Promise<void> => {
        try {
            await Torrent.upsert(torrent);
            await this.createContents(torrent.infoHash, torrent.contents);
            await this.createSubtitles(torrent.infoHash, torrent.subtitles);
        } catch (error) {
            this.logger.error(`Failed to create torrent: ${torrent.infoHash}`);
            this.logger.debug("Error: ", error);
        }
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

    public createFile = async (file: IFileCreationAttributes): Promise<void> => {
        try {
            const operatingFile = File.build(file);
            if (operatingFile.id) {
                if (operatingFile.dataValues) {
                    await operatingFile.save();
                } else {
                    await File.upsert(file);
                }
                await this.upsertSubtitles(operatingFile, file.subtitles);
            } else {
                if (operatingFile.subtitles && operatingFile.subtitles.length) {
                    operatingFile.subtitles = operatingFile.subtitles.map(subtitle => {
                        subtitle.title = subtitle.path || '';
                        return subtitle;
                    });
                }
                await File.create(file, {include: [Subtitle], ignoreDuplicates: true});
            }
        } catch (error) {
            this.logger.error(`Failed to create file: ${file.infoHash}`);
            this.logger.debug("Error: ", error);
        }
    };

    public getFiles = async (infoHash: string): Promise<File[]> => File.findAll({where: {infoHash: infoHash}});

    public getFilesBasedOnTitle = async (titleQuery: string): Promise<File[]> => File.findAll({where: {title: {[Op.regexp]: `${titleQuery}`}}});

    public deleteFile = async (id: number): Promise<number> => File.destroy({where: {id: id}});

    public createSubtitles = async (infoHash: string, subtitles: ISubtitleCreationAttributes[] | undefined): Promise<void | Model<ISubtitleAttributes, ISubtitleCreationAttributes>[]> => {
        if (subtitles && subtitles.length) {
            return Subtitle.bulkCreate(subtitles.map(subtitle => ({...subtitle, infoHash: infoHash, title: subtitle.path})));
        }
        return Promise.resolve();
    };

    public upsertSubtitles = async (file: File, subtitles: ISubtitleCreationAttributes[] | undefined): Promise<void> => {
        if (file.id && subtitles && subtitles.length) {
            await PromiseHelpers.sequence(subtitles
                .map(subtitle => {
                    subtitle.fileId = file.id;
                    subtitle.infoHash = subtitle.infoHash || file.infoHash;
                    subtitle.title = subtitle.title || subtitle.path || '';
                    return subtitle;
                })
                .map(subtitle => async () => {
                    const operatingInstance = Subtitle.build(subtitle);
                    if (operatingInstance.dataValues) {
                        await operatingInstance.save();
                    } else {
                        await Subtitle.create(subtitle);
                    }
                }));
        }
    };

    public getSubtitles = async (infoHash: string): Promise<Subtitle[]> => Subtitle.findAll({where: {infoHash: infoHash}});

    public getUnassignedSubtitles = async (): Promise<Subtitle[]> => Subtitle.findAll({where: {fileId: null}});

    public createContents = async (infoHash: string, contents: IContentCreationAttributes[] | undefined): Promise<void> => {
        if (contents && contents.length) {
            await Content.bulkCreate(contents.map(content => ({...content, infoHash})), {ignoreDuplicates: true});
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

    public createSkipTorrent = async (torrent: ITorrentCreationAttributes): Promise<[SkipTorrent, boolean | null]> => SkipTorrent.upsert({infoHash: torrent.infoHash});

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
