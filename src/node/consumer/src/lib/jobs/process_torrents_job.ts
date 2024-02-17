import {IIngestedRabbitMessage, IIngestedRabbitTorrent} from "@interfaces/ingested_rabbit_message";
import {ILoggingService} from "@interfaces/logging_service";
import {IProcessTorrentsJob} from "@interfaces/process_torrents_job";
import {ITorrentProcessingService} from "@interfaces/torrent_processing_service";
import {IIngestedTorrentAttributes} from "@repository/interfaces/ingested_torrent_attributes";
import {configurationService} from '@services/configuration_service';
import {IocTypes} from "@setup/ioc_types";
import client, {Channel, Connection, ConsumeMessage, Options} from 'amqplib'
import {inject, injectable} from "inversify";

@injectable()
export class ProcessTorrentsJob implements IProcessTorrentsJob {
    @inject(IocTypes.ITorrentProcessingService) torrentProcessingService: ITorrentProcessingService;
    @inject(IocTypes.ILoggingService) logger: ILoggingService;

    private readonly assertQueueOptions: Options.AssertQueue = {durable: true};
    private readonly consumeQueueOptions: Options.Consume = {noAck: false};

    async listenToQueue(): Promise<void> {
        if (!configurationService.jobConfig.JOBS_ENABLED) {
            return;
        }

        try {
            const connection: Connection = await client.connect(configurationService.rabbitConfig.RABBIT_URI);
            const channel: Channel = await connection.createChannel();
            await this.assertAndConsumeQueue(channel);
        } catch (error) {
            this.logger.error('Failed to connect and setup channel', error);
        }
    }

    private processMessage = (msg: ConsumeMessage | null): Promise<void> => {
        const ingestedTorrent: IIngestedTorrentAttributes = this.getMessageAsJson(msg);
        return this.torrentProcessingService.processTorrentRecord(ingestedTorrent);
    };

    private getMessageAsJson = (msg: ConsumeMessage | null): IIngestedTorrentAttributes => {
        const content = msg?.content.toString('utf8') ?? "{}";
        const receivedObject: IIngestedRabbitMessage = JSON.parse(content);
        const receivedTorrent: IIngestedRabbitTorrent = receivedObject.message;
        return {...receivedTorrent, info_hash: receivedTorrent.infoHash};
    };

    private assertAndConsumeQueue = async (channel: Channel): Promise<void> => {
        this.logger.info('Worker is running! Waiting for new torrents...');

        const ackMsg = async (msg: ConsumeMessage | null): Promise<void> => {
            await this.processMessage(msg)
                .then(() => this.logger.info('Processed torrent'))
                .then(() => msg && channel.ack(msg))
                .catch((error) => this.logger.error('Failed to process torrent', error));
        }

        try {
            await channel.assertQueue(configurationService.rabbitConfig.QUEUE_NAME, this.assertQueueOptions);
            await channel.prefetch(configurationService.jobConfig.JOB_CONCURRENCY);
            await channel.consume(configurationService.rabbitConfig.QUEUE_NAME, ackMsg, this.consumeQueueOptions);
        } catch (error) {
            this.logger.error('Failed to setup channel', error);
        }
    };
}