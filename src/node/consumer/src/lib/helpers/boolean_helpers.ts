export class BooleanHelpers {
    public static parseBool(value: string, defaultValue: boolean): boolean {
        if (value === undefined) {
            return defaultValue;
        }
        return value.toLowerCase() === 'true';
    }
}