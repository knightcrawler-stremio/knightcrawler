import {inject, injectable} from "inversify";
import WebTorrent from "webtorrent";
import {IWebTorrentService} from "@interfaces/webtorrent_service";
import {IocTypes} from "@setup/ioc_types";
import {ILoggingService} from "@interfaces/logging_service";
import * as process from "process";
import {ITorrentFile} from "@interfaces/torrent_file";
import {ITrackerService} from "@interfaces/tracker_service";
import {configurationService} from "@services/configuration_service";

const client = new WebTorrent({maxConns: configurationService.torrentConfig.MAX_CONNECTIONS_PER_TORRENT});

@injectable()
export class WebTorrentService implements IWebTorrentService {
    @inject(IocTypes.ILoggingService) private logger: ILoggingService;
    @inject(IocTypes.ITrackerService) trackerService: ITrackerService;
    constructor() {
        client.on('error', (err) => {
            this.logger.debug("WebTorrent client - Error", err);
            this.logger.error("WebTorrent client - Killing Service")
            process.exit(1);
        })
    }

    async getTorrentContents(magnet: string, timeoutWindow: number): Promise<ITorrentFile[]> {
        return new Promise(async (resolve, reject) => {
            const trackers = await this.trackerService.getTrackers();
            const torrent = client.add(magnet, {destroyStoreOnDestroy: true, announce: trackers},
                (torrent) => {
                    this.logger.debug("Torrent added", torrent.name);
                });

            const timeout = setTimeout(() => {
                this.destroyTorrent(torrent);
                reject(new Error("Timeout while loading torrent"));
            }, timeoutWindow);

            torrent.on('ready', () => {
                this.logger.debug("Torrent ready", torrent.name)
                const files = torrent.files.map((file, index) => ({
                    name: file.name,
                    path: file.path,
                    length: file.length,
                    fileIndex: index,
                }));
                clearTimeout(timeout);
                this.destroyTorrent(torrent);
                resolve(files);
            })

            torrent.on('error', (e) => {
                clearTimeout(timeout);
                this.logger.debug("Error while loading torrent", e);
                this.logger.error("Error while loading torrent")
                reject(e);
            })
        })
    }

    destroyTorrent(torrent: WebTorrent.Torrent): void {
        torrent.destroy({ destroyStore: true }, (err) => {
            if (err) {
                this.logger.error("Error while destroying torrent:", err);
                process.exit(1);
            } else {
                this.logger.debug(`Torrent destroyed ${torrent.infoHash}`);
            }
        });
    }
}
