import amqp from 'amqplib'
import { rabbitConfig, jobConfig } from '../lib/config.js'
import { processTorrentRecord } from "../lib/ingestedTorrent.js";
import {logger} from "../lib/logger";

const assertQueueOptions = { durable: true }
const consumeQueueOptions = { noAck: false }

const processMessage = msg => processTorrentRecord(getMessageAsJson(msg));

const getMessageAsJson = msg => 
    JSON.parse(msg.content.toString()).message;

const assertAndConsumeQueue = async channel => {
    logger.info('Worker is running! Waiting for new torrents...')

    const ackMsg = msg => 
    processMessage(msg)
        .then(() => channel.ack(msg))
        .catch(error => logger.error('Failed processing torrent', error));

    channel.assertQueue(rabbitConfig.QUEUE_NAME, assertQueueOptions)
        .then(() => channel.prefetch(jobConfig.JOB_CONCURRENCY))
        .then(() => channel.consume(rabbitConfig.QUEUE_NAME, ackMsg, consumeQueueOptions))
        .catch(error => logger.error('Failed to setup channel', error));
}

export const listenToQueue = async () => {
        if (!jobConfig.JOBS_ENABLED) {
            return;
        }

        return amqp.connect(rabbitConfig.URI)
            .then(connection => connection.createChannel())
            .then(channel => assertAndConsumeQueue(channel))
            .catch(error => logger.error('Failed to connect and setup channel', error));
    };