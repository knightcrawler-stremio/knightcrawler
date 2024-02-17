import {ISkipTorrentAttributes, ISkipTorrentCreationAttributes} from "@repository/interfaces/skip_torrent_attributes";
import {Column, DataType, Model, Table} from 'sequelize-typescript';

@Table({modelName: 'skip_torrent', timestamps: false})
export class SkipTorrent extends Model<ISkipTorrentAttributes, ISkipTorrentCreationAttributes> {

    @Column({type: DataType.STRING(64), primaryKey: true})
    declare infoHash: string;
}