import { BooleanHelpers } from '@helpers/boolean_helpers';

describe('BooleanHelpers.parseBool', () => {
    it('should return true when value is "true"', () => {
        expect(BooleanHelpers.parseBool('true', false)).toBe(true);
    });

    it('should return true when value is "1"', () => {
        expect(BooleanHelpers.parseBool('1', false)).toBe(true);
    });

    it('should return true when value is "yes"', () => {
        expect(BooleanHelpers.parseBool('yes', false)).toBe(true);
    });

    it('should return false when value is "false"', () => {
        expect(BooleanHelpers.parseBool('false', true)).toBe(false);
    });

    it('should return false when value is "0"', () => {
        expect(BooleanHelpers.parseBool('0', true)).toBe(false);
    });

    it('should return false when value is "no"', () => {
        expect(BooleanHelpers.parseBool('no', true)).toBe(false);
    });

    it('should return default value when value is undefined', () => {
        expect(BooleanHelpers.parseBool(undefined, true)).toBe(true);
    });
});
