import {Table, Column, Model, HasMany, DataType, BelongsTo, ForeignKey} from 'sequelize-typescript';
import {ContentAttributes, ContentCreationAttributes} from "../interfaces/content_attributes";
import {Torrent} from "./torrent";

@Table({modelName: 'content', timestamps: false})
export class Content extends Model<ContentAttributes, ContentCreationAttributes> {
    @Column({ type: DataType.STRING(64), primaryKey: true, allowNull: false, onDelete: 'CASCADE' })
    @ForeignKey(() => Torrent)
    declare infoHash: string;
    
    @Column({ type: DataType.INTEGER, primaryKey: true, allowNull: false })
    declare fileIndex: number;
    
    @Column({ type: DataType.STRING(512), allowNull: false })
    declare path: string;
    
    @Column({ type: DataType.BIGINT })
    declare size: number;

    @BelongsTo(() => Torrent, { constraints: false, foreignKey: 'infoHash' })
    torrent: Torrent;
}