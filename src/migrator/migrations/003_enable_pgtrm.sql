CREATE EXTENSION if not exists pg_trgm;
SET pg_trgm.similarity_threshold = 0.5;