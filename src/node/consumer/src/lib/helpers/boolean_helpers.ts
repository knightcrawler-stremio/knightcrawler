export const BooleanHelpers = {
    parseBool: function(value: string | undefined, defaultValue: boolean) {
        if (value === undefined) {
            return defaultValue;
        }
        return value.toLowerCase().trim() === 'true';
    }
}