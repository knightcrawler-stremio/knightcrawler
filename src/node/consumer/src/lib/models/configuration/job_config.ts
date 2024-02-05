import {BooleanHelpers} from "../../helpers/boolean_helpers";

export class JobConfig {
    public JOB_CONCURRENCY: number = parseInt(process.env.JOB_CONCURRENCY || "1", 10);
    public JOBS_ENABLED: boolean = BooleanHelpers.parseBool(process.env.JOBS_ENABLED, true);
}