import {BooleanHelpers} from "@helpers/boolean_helpers";

export const cacheConfig = {
    NO_CACHE: BooleanHelpers.parseBool(process.env.NO_CACHE, false),
};
