import { rabbitConfig, jobConfig } from '../lib/config.js'
import { processTorrentRecord } from "../lib/ingestedTorrent.js";
import amqp from 'amqplib'
import Promise from 'bluebird'

const assertQueueOptions = { durable: true }
const consumeQueueOptions = { noAck: false }

const processMessage = msg =>
    Promise.resolve(getMessageAsJson(msg))
        .then(torrent => processTorrentRecord(torrent))
        .then(() => Promise.resolve(msg));
        
const getMessageAsJson = msg => {
    const torrent = JSON.parse(msg.content.toString());
    return Promise.resolve(torrent.message);
}

const assertAndConsumeQueue = channel => {
    console.log('Worker is running! Waiting for new torrents...')

    const ackMsg = msg => Promise.resolve(msg)
        .then(msg => processMessage(msg))
        .then(msg => channel.ack(msg))
        .catch(error => console.error('Failed processing torrent', error));

    return channel.assertQueue(rabbitConfig.QUEUE_NAME, assertQueueOptions)
        .then(() => channel.prefetch(jobConfig.JOB_CONCURRENCY))
        .then(() => channel.consume(rabbitConfig.QUEUE_NAME, ackMsg, consumeQueueOptions))
}

export const listenToQueue = () => amqp.connect(rabbitConfig.URI)
    .then(connection => connection.createChannel())
    .then(channel => assertAndConsumeQueue(channel))