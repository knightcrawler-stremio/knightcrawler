CREATE OR REPLACE FUNCTION search_imdb_meta(search_term TEXT, category_param TEXT DEFAULT NULL, year_param TEXT DEFAULT NULL, limit_param INT DEFAULT 10)
RETURNS TABLE(imdb_id character varying(16), title character varying(1000),category character varying(50),year character varying(10), score REAL) AS $$
BEGIN
  RETURN QUERY
  SELECT imdb_metadata.imdb_id, imdb_metadata.title, imdb_metadata.category, imdb_metadata.year, similarity(imdb_metadata.title, search_term) as score
  FROM imdb_metadata
  WHERE (imdb_metadata.title % search_term)
    AND (imdb_metadata.adult = FALSE)
    AND (category_param IS NULL OR imdb_metadata.category = category_param)
    AND (year_param IS NULL OR imdb_metadata.year = year_param)
  ORDER BY score DESC
  LIMIT limit_param;
END; $$
LANGUAGE plpgsql;