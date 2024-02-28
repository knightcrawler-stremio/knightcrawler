import {TorrentType} from "@enums/torrent_types";
import {ILoggingService} from "@interfaces/logging_service";
import {IMongoMetadataQuery} from "@mongo/interfaces/mongo_metadata_query";
import {IMongoRepository} from "@mongo/interfaces/mongo_repository";
import {ImdbEntryModel} from "@mongo/models/imdb_entries_model";
import {configurationService} from '@services/configuration_service';
import {IocTypes} from "@setup/ioc_types";
import {inject, injectable} from "inversify";
import mongoose from 'mongoose';

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
            this.logger.debug('Failed to connect to mongo db', error);
            this.logger.error('Failed to connect to mongo db');
            process.exit(1);
        }
    }

    async getImdbId(title: string, category: string, year?: string | number) : Promise<string | null> {
        const seriesTypes : string[] = ['tvSeries'];
        const movieTypes : string[] = ['movie', 'tvMovie'];

        let titleTypes: string[] = [];

        if (category === TorrentType.Series) {
            titleTypes = seriesTypes;
        } else if (category === TorrentType.Movie) {
            titleTypes = movieTypes;
        }
                
        const query: IMongoMetadataQuery = {
            PrimaryTitle: { $regex: new RegExp(title, 'i') },
            TitleType: {$in: titleTypes}
        };
        
        if (year) {
            query.StartYear = year.toString();
        }

        try {
            const result = await ImdbEntryModel.findOne(query, '_id').maxTimeMS(30000);
            return result ? result._id : null;
        } catch (error) {
            this.logger.error('Query exceeded the 30 seconds time limit', error);
            return null;
        }
    }
}