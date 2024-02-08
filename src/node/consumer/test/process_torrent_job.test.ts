import "reflect-metadata"; // required
import { ILoggingService } from '@interfaces/logging_service';
import { ITorrentProcessingService } from '@interfaces/torrent_processing_service';
import { ProcessTorrentsJob } from '@jobs/process_torrents_job';
import { configurationService } from '@services/configuration_service';
import client, {ConsumeMessage} from 'amqplib';

jest.mock('@services/configuration_service', () => {
    return {
        configurationService: {
            rabbitConfig: {
                RABBIT_URI: 'amqp://localhost',
                QUEUE_NAME: 'test_queue',
            },
            jobConfig: {
                JOBS_ENABLED: true,
                JOB_CONCURRENCY: 1,
            },
        }
    }
});

jest.mock('amqplib', () => {
    return {
        connect: jest.fn().mockResolvedValue({
            createChannel: jest.fn().mockResolvedValue({
                assertQueue: jest.fn(),
                prefetch: jest.fn(),
                consume: jest.fn(),
                ack: jest.fn(),
            }),
        }),
    };
});

jest.mock('@services/logging_service', () => {
    return {
        error: jest.fn(),
        info: jest.fn(),
        debug: jest.fn(),
    }
})

jest.mock('@services/torrent_processing_service', () => {
    return {
        processTorrentRecord: jest.fn().mockResolvedValue(undefined),
    }
})

describe('ProcessTorrentsJob', () => {
   let  processTorrentsJob: ProcessTorrentsJob,
        loggingService: ILoggingService,
        torrentProcessingService: ITorrentProcessingService;

    beforeEach(() => {
        loggingService = jest.requireMock<ILoggingService>('@services/logging_service');
        torrentProcessingService = jest.requireMock('@services/torrent_processing_service');
        processTorrentsJob = new ProcessTorrentsJob(torrentProcessingService, loggingService);
    });

    afterEach(() => {
        jest.clearAllMocks()
      })

    describe('listenToQueue', () => {
        test('should connect to the rabbitmq server and create a channel', async () => {
            await processTorrentsJob.listenToQueue();
            expect(client.connect).toHaveBeenCalledWith(configurationService.rabbitConfig.RABBIT_URI);
        });

        test('should log an error if connection or channel setup fails', async () => {
            (client.connect as any).mockImplementationOnce(() => {
                throw new Error('Connection error');
            });

            await processTorrentsJob.listenToQueue();
            expect(loggingService.error).toHaveBeenCalledWith('Failed to connect and setup channel', expect.any(Error));
        });

        test('should process messages from the queue', async () => {
            const mockMessage = {
                content: Buffer.from(JSON.stringify({ 
                    message: {
                        name: 'test_name',
                        source: 'test_source',
                        category: 'test_category',
                        infoHash: 'test_hash',
                        size: 'test_size',
                        seeders: 0,
                        leechers: 0,
                        imdb: 'test_imdb',
                        processed: false,
                    } 
                })),
            } as ConsumeMessage;

            (client.connect as jest.Mock).mockResolvedValue({
                createChannel: jest.fn().mockResolvedValue({
                    assertQueue: jest.fn().mockResolvedValue({
                        consumerCount: 1,
                    }),
                    prefetch: jest.fn(),
                    consume: jest.fn().mockImplementation((_, callback) => {
                        callback(mockMessage);
                    }),
                    ack: jest.fn(),
                }),
            });

            await processTorrentsJob.listenToQueue();
            expect(loggingService.info).toHaveBeenCalledWith('Worker is running! Waiting for new torrents...');
            expect(client.connect).toHaveBeenCalledWith(configurationService.rabbitConfig.RABBIT_URI);
            expect(loggingService.error).toBeCalledTimes(0);

            expect(torrentProcessingService.processTorrentRecord).toHaveBeenCalledWith({
                name: 'test_name',
                source: 'test_source',
                category: 'test_category',
                infoHash: 'test_hash',
                size: 'test_size',
                seeders: 0,
                leechers: 0,
                imdb: 'test_imdb',
                processed: false,
                info_hash: 'test_hash'
            });
        });
    });
});