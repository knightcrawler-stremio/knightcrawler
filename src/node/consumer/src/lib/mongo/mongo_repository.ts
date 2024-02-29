import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {IImdbEntry} from "@mongo/interfaces/imdb_entry_attributes";
import {IMongoMetadataQuery} from "@mongo/interfaces/mongo_metadata_query";
import {IMongoRepository} from "@mongo/interfaces/mongo_repository";
import {ImdbEntryModel} from "@mongo/models/imdb_entries_model";
import {configurationService} from '@services/configuration_service';
import {IocTypes} from "@setup/ioc_types";
import Fuse, {FuseResult, IFuseOptions} from 'fuse.js';
import {inject, injectable} from "inversify";
import mongoose from 'mongoose';

const fuseOptions : IFuseOptions<IImdbEntry> = {
    includeScore: true,
    keys: ['PrimaryTitle', 'OriginalTitle'],
    threshold: 0.25,
};

@injectable()
export class MongoRepository implements IMongoRepository {
    @inject(IocTypes.ILoggingService) private logger: ILoggingService;
    private db: typeof mongoose = mongoose;
    
    async connect() : Promise<void> {
        try {
            await this.db.connect(configurationService.cacheConfig.MONGO_URI, {directConnection: true});
            this.logger.info('Successfully connected to mongo db');
        }
        catch (error) {
            const FAILED_TO_CONNECT = 'Failed to connect to mongo db';
            this.logger.debug(FAILED_TO_CONNECT, error);
            this.logger.error(FAILED_TO_CONNECT);
            process.exit(1);
        }
    }

    async getImdbId(title: string, category: string, year?: string | number) : Promise<string | null> {
        const titleType: string = category === TorrentType.Series ? 'tvSeries' : 'movie';
        const query: IMongoMetadataQuery = {
            $text: { $search: title },
            TitleType: titleType
        };
        if (year) {
            query.StartYear = year.toString();
        }
        try {
            const results = await ImdbEntryModel.find(query).limit(100).maxTimeMS(30000);
            if (!results.length) {
                return null;
            }
            const fuse: Fuse<IImdbEntry> = new Fuse(results, fuseOptions);
            const searchResults: FuseResult<IImdbEntry>[] = fuse.search(title);
            if (!searchResults.length) {
                return null;
            }
            const [bestMatch] = searchResults;
            return bestMatch.item._id;
        } catch (error) {
            this.logger.error('Query exceeded the 30 seconds time limit', error);
            return null;
        }
    }
}