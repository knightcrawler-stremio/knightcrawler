import {Table, Column, Model, HasMany, DataType, BelongsTo, ForeignKey} from 'sequelize-typescript';
import {FileAttributes, FileCreationAttributes} from "../interfaces/file_attributes";
import {Torrent} from "./torrent";
import {Subtitle} from "./subtitle";
import {SubtitleAttributes} from "../interfaces/subtitle_attributes";

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
    { unique: false, fields: ['imdbId', 'imdbSeason', 'imdbEpisode'] },
    { unique: false, fields: ['kitsuId', 'kitsuEpisode'] }
];

@Table({modelName: 'file', timestamps: true, indexes: indexes })
export class File extends Model<FileAttributes, FileCreationAttributes> {
    @Column({ type: DataType.STRING(64), allowNull: false, onDelete: 'CASCADE' })
    @ForeignKey(() => Torrent)
    declare infoHash: string;
    
    @Column({ type: DataType.INTEGER})
    declare fileIndex: number;
    
    @Column({ type: DataType.STRING(512), allowNull: false })
    declare title: string;
    
    @Column({ type: DataType.BIGINT })
    declare size: number;
    
    @Column({ type: DataType.STRING(32) })
    declare imdbId: string;
    
    @Column({ type: DataType.INTEGER })
    declare imdbSeason: number;
    
    @Column({ type: DataType.INTEGER })
    declare imdbEpisode: number;
    
    @Column({ type: DataType.INTEGER })
    declare kitsuId: number;
    
    @Column({ type: DataType.INTEGER })
    declare kitsuEpisode: number;

    @HasMany(() => Subtitle, { constraints: false, foreignKey: 'fileId'})
    declare subtitles?: Subtitle[];

    @BelongsTo(() => Torrent, { constraints: false, foreignKey: 'infoHash' })
    torrent: Torrent;
}