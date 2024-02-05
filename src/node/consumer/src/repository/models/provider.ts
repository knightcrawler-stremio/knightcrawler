import { Table, Column, Model, HasMany, DataType } from 'sequelize-typescript';
import {ProviderAttributes, ProviderCreationAttributes} from "../interfaces/provider_attributes";

@Table({modelName: 'provider', timestamps: false})
export class Provider extends Model<ProviderAttributes, ProviderCreationAttributes> {
    
    @Column({ type: DataType.STRING(32), primaryKey: true })
    declare name: string;
    
    @Column({ type: DataType.DATE })
    declare lastScraped: Date;
    
    @Column({ type: DataType.STRING(128) })
    declare lastScrapedId: string;
}