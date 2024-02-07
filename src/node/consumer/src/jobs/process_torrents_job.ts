import client, {Channel, Connection, ConsumeMessage, Options} from 'amqplib'
import {IIngestedRabbitMessage, IIngestedRabbitTorrent} from "../lib/interfaces/ingested_rabbit_message";
import {IIngestedTorrentAttributes} from "../repository/interfaces/ingested_torrent_attributes";
import {configurationService} from '../lib/services/configuration_service';
import {inject, injectable} from "inversify";
import {IocTypes} from "../lib/models/ioc_types";
import {ITorrentProcessingService} from "../lib/interfaces/torrent_processing_service";
import {ILoggingService} from "../lib/interfaces/logging_service";
import {IProcessTorrentsJob} from "../interfaces/process_torrents_job";

@injectable()
export class ProcessTorrentsJob implements IProcessTorrentsJob {
    private readonly assertQueueOptions: Options.AssertQueue = {durable: true};
    private readonly consumeQueueOptions: Options.Consume = {noAck: false};
    private torrentProcessingService: ITorrentProcessingService;
    private logger: ILoggingService;
    
    constructor(@inject(IocTypes.ITorrentProcessingService) torrentProcessingService: ITorrentProcessingService,
                @inject(IocTypes.ILoggingService) logger: ILoggingService){
        this.torrentProcessingService = torrentProcessingService;
        this.logger = logger;
    }

    public listenToQueue = async () => {
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
    private processMessage = (msg: ConsumeMessage) => {
        const ingestedTorrent: IIngestedTorrentAttributes = this.getMessageAsJson(msg);
        return this.torrentProcessingService.processTorrentRecord(ingestedTorrent);
    };
    private getMessageAsJson = (msg: ConsumeMessage): IIngestedTorrentAttributes => {
        const content = msg?.content.toString('utf8') ?? "{}";
        const receivedObject: IIngestedRabbitMessage = JSON.parse(content);
        const receivedTorrent: IIngestedRabbitTorrent = receivedObject.message;
        return {...receivedTorrent, info_hash: receivedTorrent.infoHash};
    };
    private assertAndConsumeQueue = async (channel: Channel) => {
        this.logger.info('Worker is running! Waiting for new torrents...');

        const ackMsg = async (msg: ConsumeMessage) => {
            try {
                await this.processMessage(msg);
                channel.ack(msg);
            } catch (error) {
                this.logger.error('Failed processing torrent', error);
            }
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