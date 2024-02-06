class ExtensionService {
    private readonly VIDEO_EXTENSIONS: string[] = [
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
    
    private readonly SUBTITLE_EXTENSIONS: string[] = [
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
    
    private readonly DISK_EXTENSIONS: string[] = [
        "iso",
        "m2ts",
        "ts",
        "vob"
    ]

    public isVideo(filename: string): boolean {
        return this.isExtension(filename, this.VIDEO_EXTENSIONS);
    }

    public isSubtitle(filename: string): boolean {
        return this.isExtension(filename, this.SUBTITLE_EXTENSIONS);
    }

    public isDisk(filename: string): boolean {
        return this.isExtension(filename, this.DISK_EXTENSIONS);
    }

    public isExtension(filename: string, extensions: string[]): boolean {
        const extensionMatch = filename.match(/\.(\w{2,4})$/);
        return extensionMatch !== null && extensions.includes(extensionMatch[1].toLowerCase());
    }
}

export const extensionService = new ExtensionService();