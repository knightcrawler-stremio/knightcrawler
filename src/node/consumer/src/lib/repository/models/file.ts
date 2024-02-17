import {IFileAttributes, IFileCreationAttributes} from "@repository/interfaces/file_attributes";
import {Subtitle} from "@repository/models/subtitle";
import {Torrent} from "@repository/models/torrent";
import {BelongsTo, Column, DataType, ForeignKey, HasMany, Model, Table} from 'sequelize-typescript';

const indexes = [
    {
        unique: true,
        name: 'files_unique_file_constraint',
        fields: [
            'infoHash',
            'fileIndex',
            'imdbId',
            'imdbSeason',
            'imdbEpisode',
            'kitsuId',
            'kitsuEpisode'
        ]
    },
    {unique: false, fields: ['imdbId', 'imdbSeason', 'imdbEpisode']},
    {unique: false, fields: ['kitsuId', 'kitsuEpisode']}
];

@Table({modelName: 'file', timestamps: true, indexes: indexes})
export class File extends Model<IFileAttributes, IFileCreationAttributes> {
    @Column({type: DataType.STRING(64), allowNull: false, onDelete: 'CASCADE'})
    @ForeignKey(() => Torrent)
    declare infoHash: string;

    @Column({type: DataType.INTEGER})
    declare fileIndex: number;

    @Column({type: DataType.STRING(512), allowNull: false})
    declare title: string;

    @Column({type: DataType.BIGINT})
    declare size: number;

    @Column({type: DataType.STRING(32)})
    declare imdbId: string;

    @Column({type: DataType.INTEGER})
    declare imdbSeason: number;

    @Column({type: DataType.INTEGER})
    declare imdbEpisode: number;

    @Column({type: DataType.INTEGER})
    declare kitsuId: number;

    @Column({type: DataType.INTEGER})
    declare kitsuEpisode: number;

    @HasMany(() => Subtitle, {constraints: false, foreignKey: 'fileId'})
    declare subtitles?: Subtitle[];

    @BelongsTo(() => Torrent, {constraints: false, foreignKey: 'infoHash'})
    torrent?: Torrent;
}