export const BooleanHelpers = {
    parseBool: (value: string | undefined, defaultValue: boolean): boolean => {
        if (value === undefined) {
            return defaultValue;
        }

        switch (value.trim().toLowerCase()) {
            case 'true':
            case 'yes':
            case '1':
                return true;
            case 'false':
            case 'no':
            case '0':
                return false;
            default:
                return defaultValue;
        }
    }
}