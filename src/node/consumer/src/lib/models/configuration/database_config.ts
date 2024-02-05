export class DatabaseConfig {
    public POSTGRES_HOST: string = process.env.POSTGRES_HOST || 'postgres';
    public POSTGRES_PORT: number = parseInt(process.env.POSTGRES_PORT || '5432');
    public POSTGRES_DB: string = process.env.POSTGRES_DB || 'knightcrawler';
    public POSTGRES_USER: string =  process.env.POSTGRES_USER || 'postgres';
    public POSTGRES_PASSWORD: string =  process.env.POSTGRES_PASSWORD || 'postgres';
    
    public get POSTGRES_URI() {
        return `postgres://${this.POSTGRES_USER}:${this.POSTGRES_PASSWORD}@${this.POSTGRES_HOST}:${this.POSTGRES_PORT}/${this.POSTGRES_DB}`;
    }
}