-- Purpose: Add the jsonb column to the ingested_torrents table to store the response from RTN
ALTER TABLE ingested_torrents
ADD COLUMN IF NOT EXISTS rtn_response jsonb;

-- Purpose: Drop torrentId column from torrents table
ALTER TABLE torrents
DROP COLUMN IF EXISTS "torrentId";

-- Purpose: Drop Trackers column from torrents table
ALTER TABLE torrents
DROP COLUMN IF EXISTS "trackers";

-- Purpose: Create a foreign key relationsship if it does not already exist between torrents and the source table ingested_torrents, but do not cascade on delete.
ALTER TABLE torrents
ADD COLUMN IF NOT EXISTS ingestedTorrentId bigint;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM   information_schema.table_constraints 
        WHERE  constraint_name = 'fk_torrents_info_hash'
    )
    THEN
        ALTER TABLE torrents
        DROP CONSTRAINT fk_torrents_info_hash;
    END IF;
END $$;

ALTER TABLE torrents
ADD CONSTRAINT fk_torrents_info_hash
FOREIGN KEY ("ingestedTorrentId")
REFERENCES ingested_torrents("id")
ON DELETE NO ACTION;

UPDATE torrents
SET "ingestedTorrentId" = ingested_torrents."id"
FROM ingested_torrents
WHERE torrents."infoHash" = ingested_torrents."info_hash"
AND torrents."provider" = ingested_torrents."source";