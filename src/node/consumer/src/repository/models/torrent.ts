import { Table, Column, Model, HasMany, DataType } from 'sequelize-typescript';
import {ITorrentAttributes, ITorrentCreationAttributes} from "../interfaces/torrent_attributes";
import {Content} from "./content";
import {File} from "./file";
import {Subtitle} from "./subtitle";

@Table({modelName: 'torrent', timestamps: true})

export class Torrent extends Model<ITorrentAttributes, ITorrentCreationAttributes> {
    @Column({type: DataType.STRING(64), primaryKey: true})
    declare infoHash: string;
    
    @Column({type: DataType.STRING(32), allowNull: false})
    declare provider: string;
    
    @Column({type: DataType.STRING(512)})
    declare torrentId: string;
    
    @Column({type: DataType.STRING(512), allowNull: false})
    declare title: string;
    
    @Column({type: DataType.BIGINT})
    declare size: number;
    
    @Column({type: DataType.STRING(16), allowNull: false})
    declare type: string;
    
    @Column({type: DataType.DATE, allowNull: false})
    declare uploadDate: Date;
    
    @Column({type: DataType.SMALLINT})
    declare seeders: number;
    
    @Column({type: DataType.STRING(8000)})
    declare trackers: string;
    
    @Column({type: DataType.STRING(4096)})
    declare languages: string;
    
    @Column({type: DataType.STRING(16)})
    declare resolution: string;
    
    @Column({type: DataType.BOOLEAN, allowNull: false, defaultValue: false})
    declare reviewed: boolean;
    
    @Column({type: DataType.BOOLEAN, allowNull: false, defaultValue: false})
    declare opened: boolean;

    @HasMany(() => Content, { foreignKey: 'infoHash', constraints: false })
    contents?: Content[];

    @HasMany(() => File, { foreignKey: 'infoHash', constraints: false })
    files?: File[];
    
    subtitles?: Subtitle[];
}