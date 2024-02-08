import "reflect-metadata"; // required
import { ICacheService } from '@interfaces/cache_service';
import { ILoggingService } from '@interfaces/logging_service';
import { TrackerService } from '@services/tracker_service';
import { http, HttpResponse } from 'msw'
import { setupServer } from 'msw/node';

const server = setupServer(
    http.get('https://ngosang.github.io/trackerslist/trackers_all.txt', ({ request, params, cookies }) => {
        return HttpResponse.text('http://tracker1.com\nhttp://tracker2.com')
    }),
)

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
        trackerService = new TrackerService(mockCacheService, mockLoggingService);
    });

    it('should get trackers', async () => {
        const mockTrackers = ['http://tracker1.com', 'http://tracker2.com'];

        const result = await trackerService.getTrackers();

        expect(result).toEqual(mockTrackers);
        expect(mockLoggingService.info).toHaveBeenCalledWith(`Trackers updated at 1234567890: ${mockTrackers.length} trackers`);
        
    });
});