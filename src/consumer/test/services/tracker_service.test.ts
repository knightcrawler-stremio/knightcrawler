import "reflect-metadata"; // required
import {ICacheService} from '@interfaces/cache_service';
import {ILoggingService} from '@interfaces/logging_service';
import {TrackerService} from '@services/tracker_service';
import {IocTypes} from "@setup/ioc_types";
import {Container} from "inversify";
import {setupServer} from 'msw/node';
import * as responses from "../mock-responses/trackers_mock_responses";

const server = setupServer(responses.trackerTestResponse);

jest.mock('@services/logging_service', () => {
    return {
        error: jest.fn(),
        info: jest.fn(),
        debug: jest.fn(),
    }
})

jest.mock('@services/cache_service', () => {
    return {
        cacheTrackers: jest.fn().mockImplementation((fn) => fn()),
    }
})

beforeAll(() => server.listen())
beforeEach(() => {
    jest.clearAllMocks();
    jest.spyOn(Date, 'now').mockImplementation(() => 1234567890);
})
afterEach(() => () => {
    server.resetHandlers()
    jest.spyOn(Date, 'now').mockRestore();
})
afterAll(() => server.close())

describe('TrackerService', () => {
    let trackerService: TrackerService,
        mockCacheService: ICacheService,
        mockLoggingService: ILoggingService;

    beforeEach(() => {
        mockCacheService = jest.requireMock<ICacheService>('@services/cache_service');
        mockLoggingService = jest.requireMock<ILoggingService>('@services/logging_service');

        const container = new Container();
        container.bind<TrackerService>(TrackerService).toSelf();
        container.bind<ILoggingService>(IocTypes.ILoggingService).toConstantValue(mockLoggingService);
        container.bind<ICacheService>(IocTypes.ICacheService).toConstantValue(mockCacheService);
        trackerService = container.get(TrackerService);
    });

    it('should get trackers', async () => {
        const mockTrackers = ['http://tracker1.com', 'http://tracker2.com'];

        const result = await trackerService.getTrackers();

        expect(result).toEqual(mockTrackers);
        expect(mockLoggingService.info).toHaveBeenCalledWith(`Trackers updated at 1234567890: ${mockTrackers.length} trackers`);

    });
});
