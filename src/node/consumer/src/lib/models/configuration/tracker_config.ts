import {BooleanHelpers} from "../../helpers/boolean_helpers";

export class TrackerConfig {
    public TRACKERS_URL: string = process.env.TRACKERS_URL || 'https://ngosang.github.io/trackerslist/trackers_all.txt';
    public UDP_ENABLED: boolean = BooleanHelpers.parseBool(process.env.UDP_TRACKERS_ENABLED, false);
}