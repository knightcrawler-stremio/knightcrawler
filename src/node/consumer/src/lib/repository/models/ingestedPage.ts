import {Column, DataType, Model, Table} from 'sequelize-typescript';
import {IIngestedPageAttributes, IIngestedPageCreationAttributes} from "../interfaces/ingested_page_attributes";

const indexes = [
    {
        unique: true,
        name: 'ingested_page_unique_url_constraint',
        fields: ['url']
    }
];

@Table({modelName: 'ingested_page', timestamps: true, indexes: indexes})
export class IngestedPage extends Model<IIngestedPageAttributes, IIngestedPageCreationAttributes> {
    @Column({type: DataType.STRING(512), allowNull: false})
    declare url: string;
}