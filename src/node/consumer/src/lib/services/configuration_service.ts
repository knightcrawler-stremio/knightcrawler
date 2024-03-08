import {cacheConfig} from "@models/configuration/cache_config";
import {databaseConfig} from "@models/configuration/database_config";
import {jobConfig} from "@models/configuration/job_config";
import {metadataConfig} from "@models/configuration/metadata_config";
import {rabbitConfig} from "@models/configuration/rabbit_config";
import {torrentConfig} from "@models/configuration/torrent_config";
import {trackerConfig} from "@models/configuration/tracker_config";

export const configurationService = {
    rabbitConfig: rabbitConfig,
    cacheConfig: cacheConfig,
    databaseConfig: databaseConfig,
    jobConfig: jobConfig,
    metadataConfig: metadataConfig,
    trackerConfig: trackerConfig,
    torrentConfig: torrentConfig
};
