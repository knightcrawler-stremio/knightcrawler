import {BooleanHelpers} from "../../helpers/boolean_helpers";

export const jobConfig = {
    JOB_CONCURRENCY: parseInt(process.env.JOB_CONCURRENCY || "1", 10),
    JOBS_ENABLED: BooleanHelpers.parseBool(process.env.JOBS_ENABLED, true)
};