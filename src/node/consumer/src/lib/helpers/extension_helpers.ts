const VIDEO_EXTENSIONS = [
    "3g2",
    "3gp",
    "avi",
    "flv",
    "mkv",
    "mk3d",
    "mov",
    "mp2",
    "mp4",
    "m4v",
    "mpe",
    "mpeg",
    "mpg",
    "mpv",
    "webm",
    "wmv",
    "ogm",
    "divx"
];

const SUBTITLE_EXTENSIONS = [
    "aqt",
    "gsub",
    "jss",
    "sub",
    "ttxt",
    "pjs",
    "psb",
    "rt",
    "smi",
    "slt",
    "ssf",
    "srt",
    "ssa",
    "ass",
    "usf",
    "idx",
    "vtt"
];

const DISK_EXTENSIONS = [
    "iso",
    "m2ts",
    "ts",
    "vob"
];

export const ExtensionHelpers = {
    isVideo(filename: string): boolean {
        return this.isExtension(filename, VIDEO_EXTENSIONS);
    },

    isSubtitle(filename: string): boolean {
        return this.isExtension(filename, SUBTITLE_EXTENSIONS);
    },

    isDisk(filename: string): boolean {
        return this.isExtension(filename, DISK_EXTENSIONS);
    },

    isExtension(filename: string, extensions: string[]): boolean {
        const extensionMatch = filename.match(/\.(\w{2,4})$/);
        return extensionMatch !== null && extensions.includes(extensionMatch[1].toLowerCase());
    }
}
