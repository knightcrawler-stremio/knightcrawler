-- Drop Duplicate Files in Files Table
DELETE FROM public.files
WHERE id NOT IN (
    SELECT MAX(id)
    FROM public.files
    GROUP BY "infoHash", "fileIndex"
);

-- Add Index to files table
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'files_unique_infohash_fileindex'
    ) THEN
        ALTER TABLE public.files
            ADD CONSTRAINT files_unique_infohash_fileindex UNIQUE ("infoHash", "fileIndex");
    END IF;
END $$;


-- Drop Duplicate subtitles in Subtitles Table
DELETE FROM public.subtitles
WHERE id NOT IN (
    SELECT MAX(id)
    FROM public.subtitles
    GROUP BY "infoHash", "fileIndex"
);

-- Add Index to subtitles table
DO $$
    BEGIN
        IF NOT EXISTS (
            SELECT 1
            FROM pg_constraint
            WHERE conname = 'subtitles_unique_infohash_fileindex'
        ) THEN
            ALTER TABLE public.subtitles
                ADD CONSTRAINT subtitles_unique_infohash_fileindex UNIQUE ("infoHash", "fileIndex");
        END IF;
    END $$;

