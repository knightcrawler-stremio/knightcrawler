import {PromiseHelpers} from '@helpers/promises_helpers';
import {ILoggingService} from "@interfaces/logging_service";
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
import {IocTypes} from "@setup/ioc_types";
import {inject, injectable} from "inversify";
import moment from 'moment';
import {literal, Op, WhereOptions} from "sequelize";
import {Model, Sequelize} from 'sequelize-typescript';
import Fuse, {FuseResult, IFuseOptions} from "fuse.js";
import {TorrentType} from "@enums/torrent_types";
import {ImdbMetadata} from "@repository/models/imdbMetadata";

const fuseOptions : IFuseOptions<ImdbMetadata> = {
    includeScore: true,
    keys: ['title'],
    threshold: configurationService.metadataConfig.TITLE_MATCH_THRESHOLD,
};

@injectable()
export class DatabaseRepository implements IDatabaseRepository {
    @inject(IocTypes.ILoggingService) logger: ILoggingService;

    private readonly database: Sequelize;

    private models = [
        Torrent,
        Provider,
        File,
        Subtitle,
        Content,
        SkipTorrent,
        IngestedTorrent,
        IngestedPage,
        ImdbMetadata];

    constructor() {
        this.database = this.initializeSequelize();
    }

    async connect(): Promise<void> {
        try {
            await this.database.authenticate();
        } catch (error) {
            this.logger.debug('Failed to authenticate database', error);
            this.logger.error('Failed to authenticate database');
            process.exit(1);
        }
    }

    async getProvider(provider: Provider): Promise<Provider> {
        try {
            const [result] = await Provider.findOrCreate({where: {name: {[Op.eq]: provider.name}}, defaults: provider});
            return result;
        } catch {
            return provider as Provider;
        }
    }

    async getTorrent(torrent: ITorrentAttributes): Promise<Torrent | null> {
        const where = torrent.infoHash
            ? {infoHash: torrent.infoHash}
            : {provider: torrent.provider, torrentId: torrent.torrentId};
        return await Torrent.findOne({where});
    }

    async getTorrentsBasedOnTitle(titleQuery: string, type: string): Promise<Torrent[]> {
        return this.getTorrentsBasedOnQuery({
            title: {[Op.regexp]: `${titleQuery}`},
            type
        });
    }

    async getTorrentsBasedOnQuery(where: WhereOptions<ITorrentAttributes>): Promise<Torrent[]> {
        return await Torrent.findAll({where});
    }

    async getFilesBasedOnQuery(where: WhereOptions<IFileAttributes>): Promise<File[]> {
        return await File.findAll({where});
    }

    async getTorrentsWithoutSize(): Promise<Torrent[]> {
        return await Torrent.findAll({
            where: literal(
                'exists (select 1 from files where files."infoHash" = torrent."infoHash" and files.size = 300000000)'),
            order: [
                ['seeders', 'DESC']
            ]
        });
    }

    async getUpdateSeedersTorrents(limit = 50): Promise<Torrent[]> {
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

    async getUpdateSeedersNewTorrents(limit = 50): Promise<Torrent[]> {
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

    async getNoContentsTorrents(): Promise<Torrent[]> {
        return await Torrent.findAll({
            where: {opened: false, seeders: {[Op.gte]: 1}},
            limit: 500,
            order: literal('random()')
        });
    }

    async createTorrent(torrent: ITorrentCreationAttributes): Promise<void> {
        try {
            await Torrent.upsert(torrent);
            await this.createContents(torrent.infoHash, torrent.contents);
            await this.createSubtitles(torrent.infoHash, torrent.subtitles);
        } catch (error) {
            this.logger.error(`Failed to create torrent: ${torrent.infoHash}`);
            this.logger.debug("Error: ", error);
        }
    }

    async setTorrentSeeders(torrent: ITorrentAttributes, seeders: number): Promise<[number]> {
        const where = torrent.infoHash
            ? {infoHash: torrent.infoHash}
            : {provider: torrent.provider, torrentId: torrent.torrentId};

        return await Torrent.update(
            {seeders: seeders},
            {where: where}
        );
    }

    async deleteTorrent(infoHash: string): Promise<number> {
        return await Torrent.destroy({where: {infoHash: infoHash}});
    }

    async createFile(file: IFileCreationAttributes): Promise<void> {
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
    }

    async getFiles(infoHash: string): Promise<File[]> {
        return File.findAll({where: {infoHash: infoHash}});
    }

    async getFilesBasedOnTitle(titleQuery: string): Promise<File[]> {
        return File.findAll({where: {title: {[Op.regexp]: `${titleQuery}`}}});
    }

    async deleteFile(id: number): Promise<number> {
        return File.destroy({where: {id: id}});
    }

    async createSubtitles(infoHash: string, subtitles: ISubtitleCreationAttributes[] | undefined): Promise<void | Model<ISubtitleAttributes, ISubtitleCreationAttributes>[]> {
        if (subtitles && subtitles.length) {
            return Subtitle.bulkCreate(subtitles.map(subtitle => ({...subtitle, infoHash: infoHash, title: subtitle.path})));
        }
        return Promise.resolve();
    }

    async upsertSubtitles(file: File, subtitles: ISubtitleCreationAttributes[] | undefined): Promise<void> {
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
    }

    async getSubtitles(infoHash: string): Promise<Subtitle[]> {
        return Subtitle.findAll({where: {infoHash: infoHash}});
    }

    async getUnassignedSubtitles(): Promise<Subtitle[]> {
        return Subtitle.findAll({where: {fileId: null}});
    }

    async createContents(infoHash: string, contents: IContentCreationAttributes[] | undefined): Promise<void> {
        if (contents && contents.length) {
            await Content.bulkCreate(contents.map(content => ({...content, infoHash})), {ignoreDuplicates: true});
            await Torrent.update({opened: true}, {where: {infoHash: infoHash}, silent: true});
        }
    }

    async getContents(infoHash: string): Promise<Content[]> {
        return Content.findAll({where: {infoHash: infoHash}});
    }

    async getSkipTorrent(infoHash: string): Promise<SkipTorrent> {
        const result = await SkipTorrent.findByPk(infoHash);
        if (!result) {
            throw new Error(`torrent not found: ${infoHash}`);
        }
        return result.dataValues as SkipTorrent;
    }

    async createSkipTorrent(torrent: ITorrentCreationAttributes): Promise<[SkipTorrent, boolean | null]> {
        return SkipTorrent.upsert({infoHash: torrent.infoHash});
    }

    async getImdbId(title: string, category: string, year?: string | number) : Promise<string | null> {
        const titleType: string = category === TorrentType.Series ? 'tvSeries' : 'movie';
        const query = `SELECT * FROM search_imdb_meta('${title}', '${titleType}'${year ? `, '${year}'` : ''})`;
        try {
            const imdbEntries: ImdbMetadata[] = await this.database.query(query, {mapToModel: true, model: ImdbMetadata});
            const fuse: Fuse<ImdbMetadata> = new Fuse(imdbEntries, fuseOptions);
            const searchResults: FuseResult<ImdbMetadata>[] = fuse.search(title);
            if (!searchResults.length) {
                return null;
            }
            const [bestMatch] = searchResults;
            return bestMatch.item.imdb_id;
        } catch (error) {
            this.logger.error('Query exceeded the 30 seconds time limit', error);
            return null;
        }
    }

    private initializeSequelize = (): Sequelize => {
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
