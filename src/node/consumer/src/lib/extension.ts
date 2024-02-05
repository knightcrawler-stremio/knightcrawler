const VIDEO_EXTENSIONS: string[] = [
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
const SUBTITLE_EXTENSIONS: string[] = [
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
const DISK_EXTENSIONS: string[] = [
    "iso",
    "m2ts",
    "ts",
    "vob"
]

export function isVideo(filename: string): boolean {
    return isExtension(filename, VIDEO_EXTENSIONS);
}

export function isSubtitle(filename: string): boolean {
    return isExtension(filename, SUBTITLE_EXTENSIONS);
}

export function isDisk(filename: string): boolean {
    return isExtension(filename, DISK_EXTENSIONS);
}

export function isExtension(filename: string, extensions: string[]): boolean {
    const extensionMatch = filename.match(/\.(\w{2,4})$/);
    return extensionMatch !== null && extensions.includes(extensionMatch[1].toLowerCase());
}