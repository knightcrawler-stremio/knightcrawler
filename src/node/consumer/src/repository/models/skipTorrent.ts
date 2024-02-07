import { Table, Column, Model, HasMany, DataType } from 'sequelize-typescript';
import {ISkipTorrentAttributes, ISkipTorrentCreationAttributes} from "../interfaces/skip_torrent_attributes";


@Table({modelName: 'skip_torrent', timestamps: false})
export class SkipTorrent extends Model<ISkipTorrentAttributes, ISkipTorrentCreationAttributes> {
    
    @Column({ type: DataType.STRING(64), primaryKey: true })
    declare infoHash: string;
}