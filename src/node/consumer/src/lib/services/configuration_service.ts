import {RabbitConfig} from "../models/configuration/rabbit_config";
import {CacheConfig} from "../models/configuration/cache_config";
import {DatabaseConfig} from "../models/configuration/database_config";
import {JobConfig} from "../models/configuration/job_config";
import {MetadataConfig} from "../models/configuration/metadata_config";
import {TrackerConfig} from "../models/configuration/tracker_config";
import {TorrentConfig} from "../models/configuration/torrent_config";

class ConfigurationService {
    public readonly rabbitConfig = new RabbitConfig();
    public readonly cacheConfig = new CacheConfig();
    public readonly databaseConfig = new DatabaseConfig();
    public readonly jobConfig = new JobConfig();
    public readonly metadataConfig = new MetadataConfig();
    public readonly trackerConfig = new TrackerConfig();
    public readonly torrentConfig = new TorrentConfig();
}

export const configurationService = new ConfigurationService();
