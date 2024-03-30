-- Remove the old search function
DROP FUNCTION IF EXISTS search_imdb_meta(TEXT, TEXT, INT, INT);

-- Add the new search function that allows for searching by year with a plus/minus one year range
CREATE OR REPLACE FUNCTION search_imdb_meta(search_term TEXT, category_param TEXT DEFAULT NULL, year_param INT DEFAULT NULL, limit_param INT DEFAULT 10, similarity_threshold REAL DEFAULT 0.95)
    RETURNS TABLE(imdb_id character varying(16), title character varying(1000),category character varying(50),year INT, score REAL) AS $$
BEGIN
    SET pg_trgm.similarity_threshold = similarity_threshold;
    RETURN QUERY
        SELECT imdb_metadata.imdb_id, imdb_metadata.title, imdb_metadata.category, imdb_metadata.year, similarity(imdb_metadata.title, search_term) as score
        FROM imdb_metadata
        WHERE (imdb_metadata.title % search_term)
          AND (imdb_metadata.adult = FALSE)
          AND (category_param IS NULL OR imdb_metadata.category = category_param)
          AND (year_param IS NULL OR imdb_metadata.year BETWEEN year_param - 1 AND year_param + 1)
        ORDER BY score DESC
        LIMIT limit_param;
END; $$
    LANGUAGE plpgsql;