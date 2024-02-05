import {Logger, pino} from "pino";

class LoggingService {
    public readonly logger: Logger = pino({
        level: process.env.LOG_LEVEL || 'info'
    });
    
    public info(message: string, ...args: any[]): void {
        this.logger.info(message);
    }
    
    public error(message: string, ...args: any[]): void {
        this.logger.error(message);
    }
    
    public debug(message: string, ...args: any[]): void {
        this.logger.debug(message);
    }
    
    public warn(message: string, ...args: any[]): void {
        this.logger.warn(message);
    }
}

export const logger = new LoggingService();

