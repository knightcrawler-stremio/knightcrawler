import {Column, DataType, Model, Table} from 'sequelize-typescript';
import {IProviderAttributes, IProviderCreationAttributes} from "../interfaces/provider_attributes";

@Table({modelName: 'provider', timestamps: false})
export class Provider extends Model<IProviderAttributes, IProviderCreationAttributes> {

    @Column({type: DataType.STRING(32), primaryKey: true})
    declare name: string;

    @Column({type: DataType.DATE})
    declare lastScraped: Date;

    @Column({type: DataType.STRING(128)})
    declare lastScrapedId: string;
}