import client, {Channel, Connection, ConsumeMessage, Options} from 'amqplib'
import {IIngestedRabbitMessage, IIngestedRabbitTorrent} from "../lib/interfaces/ingested_rabbit_message";
import {IIngestedTorrentAttributes} from "../repository/interfaces/ingested_torrent_attributes";
import {configurationService} from '../lib/services/configuration_service';
import {torrentProcessingService} from '../lib/services/torrent_processing_service';
import {logger} from '../lib/services/logging_service';

class ProcessTorrentsJob {
    private readonly assertQueueOptions: Options.AssertQueue = {durable: true};
    private readonly consumeQueueOptions: Options.Consume = {noAck: false};

    public listenToQueue = async ()=> {
        if (!configurationService.jobConfig.JOBS_ENABLED) {
            return;
        }

        try {
            const connection: Connection = await client.connect(configurationService.rabbitConfig.RABBIT_URI);
            const channel: Channel = await connection.createChannel();
            await this.assertAndConsumeQueue(channel);
        } catch (error) {
            logger.error('Failed to connect and setup channel', error);
        }
    }
    private processMessage = (msg: ConsumeMessage) => {
        const ingestedTorrent: IIngestedTorrentAttributes = this.getMessageAsJson(msg);
        return torrentProcessingService.processTorrentRecord(ingestedTorrent);
    };
    private getMessageAsJson = (msg: ConsumeMessage): IIngestedTorrentAttributes => {
        const content = msg?.content.toString('utf8') ?? "{}";
        const receivedObject: IIngestedRabbitMessage = JSON.parse(content);
        const receivedTorrent: IIngestedRabbitTorrent = receivedObject.message;
        return {...receivedTorrent, info_hash: receivedTorrent.infoHash};
    };
    private async assertAndConsumeQueue(channel: Channel) {
        logger.info('Worker is running! Waiting for new torrents...');

        const ackMsg = async (msg: ConsumeMessage) => {
            try {
                await this.processMessage(msg);
                channel.ack(msg);
            } catch (error) {
                logger.error('Failed processing torrent', error);
            }
        }
        
        try {
            await channel.assertQueue(configurationService.rabbitConfig.QUEUE_NAME, this.assertQueueOptions);
            await channel.prefetch(configurationService.jobConfig.JOB_CONCURRENCY);
            await channel.consume(configurationService.rabbitConfig.QUEUE_NAME, ackMsg, this.consumeQueueOptions);
        } catch (error) {
            logger.error('Failed to setup channel', error);
        }
    }
    
    private test() {
        
    }
}

export const processTorrentsJob = new ProcessTorrentsJob();