import {Column, DataType, Model, Table} from 'sequelize-typescript';
import {IImdbMetadataAttributes} from "@repository/interfaces/imdb_metadata_attributes";

const indexes = [
    {unique: false, fields: ['imdbId', 'category', 'title', 'year']},
];

@Table({modelName: 'imdb_metadata', timestamps: true, indexes: indexes})
export class ImdbMetadata extends Model<IImdbMetadataAttributes> {
    @Column({type: DataType.STRING(16), allowNull: false})
    declare imdb_id: string;

    @Column({type: DataType.STRING(50)})
    declare category: number;

    @Column({type: DataType.STRING(1000)})
    declare title: string;

    @Column({type: DataType.STRING(10)})
    declare year: number;

    @Column({type: DataType.BOOLEAN})
    declare adult: string;

    declare score: number;
}
