import {IContentAttributes, IContentCreationAttributes} from "@repository/interfaces/content_attributes";
import {Torrent} from "@repository/models/torrent";
import {BelongsTo, Column, DataType, ForeignKey, Model, Table} from 'sequelize-typescript';

@Table({modelName: 'content', timestamps: false})
export class Content extends Model<IContentAttributes, IContentCreationAttributes> {
    @Column({type: DataType.STRING(64), primaryKey: true, allowNull: false, onDelete: 'CASCADE'})
    @ForeignKey(() => Torrent)
    declare infoHash: string;

    @Column({type: DataType.INTEGER, primaryKey: true, allowNull: false})
    declare fileIndex: number;

    @Column({type: DataType.STRING(512), allowNull: false})
    declare path: string;

    @Column({type: DataType.BIGINT})
    declare size: number;

    @BelongsTo(() => Torrent, {constraints: false, foreignKey: 'infoHash'})
    torrent?: Torrent;
}