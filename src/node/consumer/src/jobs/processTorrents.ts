import client, {Channel, Connection, ConsumeMessage, Options} from 'amqplib'
import {jobConfig, rabbitConfig} from '../lib/config';
import {torrentProcessingService} from '../lib/services/torrent_processing_service';
import {logger} from '../lib/services/logging_service';
import {IngestedRabbitMessage, IngestedRabbitTorrent} from "../lib/interfaces/ingested_rabbit_message";
import {IngestedTorrentAttributes} from "../repository/interfaces/ingested_torrent_attributes";

const assertQueueOptions: Options.AssertQueue = { durable: true };
const consumeQueueOptions: Options.Consume = { noAck: false };

const processMessage = (msg: ConsumeMessage | null): Promise<void> => {
    const ingestedTorrent: IngestedTorrentAttributes = getMessageAsJson(msg);    
    return torrentProcessingService.processTorrentRecord(ingestedTorrent);
};

const getMessageAsJson = (msg: ConsumeMessage | null): IngestedTorrentAttributes => {
    const content = msg ? msg?.content.toString('utf8') : "{}";
    const receivedObject: IngestedRabbitMessage = JSON.parse(content) as IngestedRabbitMessage;
    const receivedTorrent:IngestedRabbitTorrent = receivedObject.message;
    const mappedObject: any = {...receivedTorrent, info_hash: receivedTorrent.infoHash};
    delete mappedObject.infoHash;
    
    return mappedObject as IngestedTorrentAttributes;
};

const assertAndConsumeQueue = async (channel: Channel): Promise<void> => {
    logger.info('Worker is running! Waiting for new torrents...');

    const ackMsg = (msg: ConsumeMessage): void => {
        processMessage(msg)
            .then(() => channel.ack(msg))
            .catch((error: Error) => logger.error('Failed processing torrent', error));
    }

    try {
        await channel.assertQueue(rabbitConfig.QUEUE_NAME, assertQueueOptions);
        await channel.prefetch(jobConfig.JOB_CONCURRENCY);
        await channel.consume(rabbitConfig.QUEUE_NAME, ackMsg, consumeQueueOptions);
    } catch(error) {
        logger.error('Failed to setup channel', error);
    }
}

export const listenToQueue = async (): Promise<void> => {
    if (!jobConfig.JOBS_ENABLED) {
        return;
    }

    try {
        const connection: Connection = await client.connect(rabbitConfig.RABBIT_URI);
        const channel: Channel = await connection.createChannel();
        await assertAndConsumeQueue(channel);
    } catch (error) {
        logger.error('Failed to connect and setup channel', error);
    }
}