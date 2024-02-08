import "reflect-metadata"; // required
import { LoggingService } from '@services/logging_service';

jest.mock('pino', () => {
    const actualPino = jest.requireActual('pino');
    return {
        ...actualPino,
        pino: jest.fn().mockImplementation(() => ({
            info: jest.fn(),
            error: jest.fn(),
            debug: jest.fn(),
            warn: jest.fn(),
        })),
    };
});

describe('LoggingService', () => {
    let service: LoggingService,
        mockLogger: any;

    beforeEach(() => {
        service = new LoggingService();
        mockLogger = (service as any).logger;
    });

    it('should log info', () => {
        service.info('test message', { key: 'value' });
        expect(mockLogger.info).toHaveBeenCalledWith('test message', [{ key: 'value' }]);
    });

    it('should log error', () => {
        service.error('test message', { key: 'value' });
        expect(mockLogger.error).toHaveBeenCalledWith('test message', [{ key: 'value' }]);
    });

    it('should log debug', () => {
        service.debug('test message', { key: 'value' });
        expect(mockLogger.debug).toHaveBeenCalledWith('test message', [{ key: 'value' }]);
    });

    it('should log warn', () => {
        service.warn('test message', { key: 'value' });
        expect(mockLogger.warn).toHaveBeenCalledWith('test message', [{ key: 'value' }]);
    })
});