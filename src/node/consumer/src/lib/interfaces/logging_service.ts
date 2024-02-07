export interface ILoggingService {
    info(message: string, ...args: any[]): void;
    error(message: string, ...args: any[]): void;
    debug(message: string, ...args: any[]): void;
    warn(message: string, ...args: any[]): void;
}