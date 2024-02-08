import "reflect-metadata"; // required
import {ICacheService} from "@interfaces/cache_service";
import {IMetadataResponse} from "@interfaces/metadata_response";
import {MetadataService} from "@services/metadata_service";
import {setupServer} from "msw/node";
import * as responses from "./mock-responses/metadata_mock_responses";

jest.mock('@services/cache_service', () => {
    return {
        cacheWrapImdbId: jest.fn().mockImplementation(async (key, fn) => await fn()),
        cacheWrapKitsuId: jest.fn().mockImplementation(async (key, fn) => await fn()),
        cacheWrapMetadata: jest.fn().mockImplementation(async (key, fn) => await fn()),
    }
})

const server = setupServer(
    responses.cinemetaQueryResponse,
    responses.cinemetaFlashMetadataSearchTestResponse,
    responses.kitsuNarutoIdSearchTestResponse,
    responses.kitsuNarutoMetaDataSearchTestResponse,
    responses.nameToImdbTheFlash,
    responses.checkIfImdbEpisode);

beforeAll(() => server.listen())
beforeEach(() => {
    jest.spyOn(Date, 'now').mockImplementation(() => 1234567890);
})
afterEach(() => () => {
    server.resetHandlers()
    jest.spyOn(Date, 'now').mockRestore();
})
afterAll(() => server.close())

describe('MetadataService Tests', () => {
    let metadataService: MetadataService,
        mockCacheService: ICacheService;

    beforeEach(() => {
        mockCacheService = jest.requireMock<ICacheService>('@services/cache_service');
        metadataService = new MetadataService(mockCacheService);
    });

    it("should get kitsu id", async () => {
        const result = await metadataService.getKitsuId({
            title: 'Naruto',
            year: 2002,
            season: 1
        });
        expect(mockCacheService.cacheWrapKitsuId).toHaveBeenCalledWith('naruto 2002 S1', expect.any(Function));
        expect(result).not.toBeNull();
        expect(result).toEqual('11');
    });

    it("should get kitsu metadata", async () => {
        const result = await metadataService.getMetadata({
            id: 'kitsu:11',
            type: 'series'
        });
        
        expect(mockCacheService.cacheWrapMetadata).toHaveBeenCalledWith('kitsu:11', expect.any(Function));
        expect(result).not.toBeNull();
        
        const body = result as IMetadataResponse;
        expect(body.videos).not.toBeNull();
        expect(body.videos.length).toBe(220);
    });

    it("should get imdb metadata", async () => {
        const result = await metadataService.getMetadata({
            id: 'tt0098798',
            type: 'series'
        });

        expect(mockCacheService.cacheWrapMetadata).toHaveBeenCalledWith('tt0098798', expect.any(Function));
        expect(result).not.toBeNull();

        const body = result as IMetadataResponse;
        expect(body.videos).not.toBeNull();
        expect(body.videos.length).toBe(22);
    });

    it("should check if imdb id is an episode", async () => {
        const result = await metadataService.isEpisodeImdbId('tt0579968');
        expect(result).toBe(true);
    });

    it("should escape title", () => {
        const result = metadataService.escapeTitle('Naruto: Shippuden');
        expect(result).toEqual('naruto shippuden');
    });
});