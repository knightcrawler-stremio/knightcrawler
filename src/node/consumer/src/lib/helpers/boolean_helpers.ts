export const BooleanHelpers = {
    parseBool: function(value: string | number | undefined, defaultValue: boolean): boolean {
        switch (typeof value) {
            case 'string':
                return parseStringToBool(value, defaultValue);
            case 'boolean':
                return value;
            case 'number':
                return parseNumberToBool(value, defaultValue);
            default:
                return defaultValue;
        }
    }
}

const parseStringToBool = (input: string, defaultVal: boolean): boolean => {
    switch (input.toLowerCase().trim()) {
        case 'true':
        case 'yes':
        case '1':
            return true;
        case 'false':
        case 'no':
        case '0':
            return false;
        default:
            return defaultVal
    }
}

const parseNumberToBool = (input: number, defaultVal: boolean): boolean => {
    switch (input) {
        case 1:
            return true;
        case 0:
            return false;
        default:
            return defaultVal;
    }
}