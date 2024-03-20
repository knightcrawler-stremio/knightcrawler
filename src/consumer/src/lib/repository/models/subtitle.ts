import {ISubtitleAttributes, ISubtitleCreationAttributes} from "@repository/interfaces/subtitle_attributes";
import {File} from "@repository/models/file";
import {BelongsTo, Column, DataType, ForeignKey, Model, Table} from 'sequelize-typescript';

const indexes = [
    {
        unique: true,
        name: 'subtitles_unique_subtitle_constraint',
        fields: [
            'infoHash',
            'fileIndex',
            'fileId'
        ]
    },
    {unique: false, fields: ['fileId']}
];

@Table({modelName: 'subtitle', timestamps: false, indexes: indexes})
export class Subtitle extends Model<ISubtitleAttributes, ISubtitleCreationAttributes> {

    @Column({type: DataType.STRING(64), allowNull: false, onDelete: 'CASCADE'})
    declare infoHash: string;

    @Column({type: DataType.INTEGER, allowNull: false})
    declare fileIndex: number;

    @Column({type: DataType.BIGINT, allowNull: true, onDelete: 'SET NULL'})
    @ForeignKey(() => File)
    declare fileId?: number | null;

    @Column({type: DataType.STRING(512), allowNull: false})
    declare title: string;

    @BelongsTo(() => File, {constraints: false, foreignKey: 'fileId'})
    file?: File;

    path?: string;
}
