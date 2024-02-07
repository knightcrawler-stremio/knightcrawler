import {Logger, pino} from "pino";
import {ILoggingService} from "../interfaces/logging_service";
import {injectable} from "inversify";

@injectable()
export class LoggingService implements ILoggingService {
    private readonly logger: Logger;

    constructor() {
        this.logger = pino({
            level: process.env.LOG_LEVEL || 'info'
        });
    }

    public info = (message: string, ...args: any[]): void => {
        this.logger.info(message, args);
    };

    public error = (message: string, ...args: any[]): void => {
        this.logger.error(message, args);
    };

    public debug = (message: string, ...args: any[]): void => {
        this.logger.debug(message, args);
    };

    public warn = (message: string, ...args: any[]): void => {
        this.logger.warn(message, args);
    };
}