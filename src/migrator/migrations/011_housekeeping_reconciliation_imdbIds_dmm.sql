DROP FUNCTION IF EXISTS kc_maintenance_reconcile_dmm_imdb_ids();
CREATE OR REPLACE FUNCTION kc_maintenance_reconcile_dmm_imdb_ids()
RETURNS INTEGER AS $$
DECLARE
    rec RECORD;
    imdb_rec RECORD;
    rows_affected INTEGER := 0;
BEGIN
    RAISE NOTICE 'Starting Reconciliation of DMM IMDB Ids...';
    FOR rec IN
        SELECT 
            it."id" as "ingestion_id",
            t."infoHash",
            it."category" as "ingestion_category",
            f."id" as "file_Id",
            f."title" as "file_Title",
            (rtn_response->>'raw_title')::text as "raw_title",
            (rtn_response->>'parsed_title')::text as "parsed_title",
            (rtn_response->>'year')::int as "year"
        FROM torrents t
        JOIN ingested_torrents it ON t."ingestedTorrentId" = it."id"
        JOIN files f ON t."infoHash" = f."infoHash"
        WHERE t."provider" = 'DMM'
    LOOP
        RAISE NOTICE 'Processing record with file_Id: %', rec."file_Id";
        FOR imdb_rec IN
            SELECT * FROM search_imdb_meta(
                rec."parsed_title",
                    CASE 
                        WHEN rec."ingestion_category" = 'tv' THEN 'tvSeries'
                        WHEN rec."ingestion_category" = 'movies' THEN 'movie' 
                    END, 
                    CASE 
                        WHEN rec."year" = 0 THEN NULL 
                        ELSE rec."year" END,
                1)
        LOOP
            IF imdb_rec IS NOT NULL THEN
                RAISE NOTICE 'Updating file_Id: % with imdbId: %, parsed title: %, imdb title: %', rec."file_Id", imdb_rec."imdb_id", rec."parsed_title", imdb_rec."title";
                UPDATE "files"
                SET "imdbId" = imdb_rec."imdb_id"
                WHERE "id" = rec."file_Id";
                rows_affected := rows_affected + 1;
            ELSE
                RAISE NOTICE 'No IMDB ID found for file_Id: %, parsed title: %, imdb title: %, setting imdbId to NULL', rec."file_Id", rec."parsed_title", imdb_rec."title";
                UPDATE "files"
                SET "imdbId" = NULL
                WHERE "id" = rec."file_Id";
            END IF;
        END LOOP;
    END LOOP;
    RAISE NOTICE 'Finished reconciliation. Total rows affected: %', rows_affected;
    RETURN rows_affected;
END;
$$ LANGUAGE plpgsql;