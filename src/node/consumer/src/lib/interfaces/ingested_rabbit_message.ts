export interface IIngestedRabbitTorrent {
    name: string;
    source: string;
    category: string;
    infoHash: string;
    size: string;
    seeders: number;
    leechers: number;
    imdb: string;
    processed: boolean;
}

export interface IIngestedRabbitMessage {
    message: IIngestedRabbitTorrent;
}