import {Document} from "mongoose";

export interface IImdbEntry extends Document {
    _id: string;
    EndYear: string;
    Genres: string;
    IsAdult: string;
    OriginalTitle: string;
    PrimaryTitle: string;
    RuntimeMinutes: string;
    StartYear: string;
    TitleType: string;
}
