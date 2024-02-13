export const BooleanHelpers = {
    parseBool: (value: string | undefined, defaultValue: boolean): boolean => {
        switch (value?.trim().toLowerCase()) {
            case undefined:
                return defaultValue;
            case 'true':
            case 'yes':
            case '1':
                return true;
            case 'false':
            case 'no':
            case '0':
                return false;
            default:
                throw new Error(`Invalid boolean value: '${value}'. Allowed values are 'true', 'false', 'yes', 'no', '1', or '0'.`);
        }
    }
}