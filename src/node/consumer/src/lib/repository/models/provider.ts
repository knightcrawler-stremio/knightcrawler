import {IProviderAttributes, IProviderCreationAttributes} from "@repository/interfaces/provider_attributes";
import {Column, DataType, Model, Table} from 'sequelize-typescript';

@Table({modelName: 'provider', timestamps: false})
export class Provider extends Model<IProviderAttributes, IProviderCreationAttributes> {

    @Column({type: DataType.STRING(32), primaryKey: true})
    declare name: string;

    @Column({type: DataType.DATE})
    declare lastScraped: Date;

    @Column({type: DataType.STRING(128)})
    declare lastScrapedId: string;
}