import { ExtensionHelpers } from '@helpers/extension_helpers';

describe('ExtensionHelpers', () => {
    describe('isVideo', () => {
        it('should return true when file extension is a video extension', () => {
            expect(ExtensionHelpers.isVideo('file.mp4')).toBe(true);
        });

        it('should return false when file extension is not a video extension', () => {
            expect(ExtensionHelpers.isVideo('file.txt')).toBe(false);
        });
    });

    describe('isSubtitle', () => {
        it('should return true when file extension is a subtitle extension', () => {
            expect(ExtensionHelpers.isSubtitle('file.srt')).toBe(true);
        });

        it('should return false when file extension is not a subtitle extension', () => {
            expect(ExtensionHelpers.isSubtitle('file.txt')).toBe(false);
        });
    });

    describe('isDisk', () => {
        it('should return true when file extension is a disk extension', () => {
            expect(ExtensionHelpers.isDisk('file.iso')).toBe(true);
        });

        it('should return false when file extension is not a disk extension', () => {
            expect(ExtensionHelpers.isDisk('file.txt')).toBe(false);
        });
    });
});
