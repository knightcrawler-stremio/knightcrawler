import {IImdbEntry} from "@mongo/interfaces/imdb_entry_attributes";
import mongoose, {Schema} from "mongoose";

const ImdbEntriesSchema: Schema = new Schema({
    _id: { type: String, required: true },
    EndYear: { type: String, default: "" },
    Genres: { type: String, default: "" },
    IsAdult: { type: String, default: "0" },
    OriginalTitle: { type: String, default: "" },
    PrimaryTitle: { type: String, required: true },
    RuntimeMinutes: { type: String, default: "" },
    StartYear: { type: String, required: true },
    TitleType: { type: String, default: "" },
});

ImdbEntriesSchema.index({ PrimaryTitle: 'text', TitleType: 1, StartYear: 1 }, { background: true });

export const ImdbEntryModel = mongoose.model<IImdbEntry>('ImdbEntry', ImdbEntriesSchema, 'imdb-entries');