import {TorrentType} from "@enums/torrent_types";
import {IMongoMetadataQuery} from "@mongo/interfaces/mongo_metadata_query";
import {IMongoRepository} from "@mongo/interfaces/mongo_repository";
import {ImdbEntryModel} from "@mongo/models/imdb_entries_model";
import {configurationService} from '@services/configuration_service';
import {injectable} from "inversify";
import mongoose from 'mongoose';

@injectable()
export class MongoRepository implements IMongoRepository {
    private db: typeof mongoose = mongoose;

    async connect() : Promise<void> {
        await this.db.connect(configurationService.cacheConfig.MONGO_URI, {directConnection: true});
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

        const result = await ImdbEntryModel.findOne(query);
        return result ? result._id : null;
    }
}