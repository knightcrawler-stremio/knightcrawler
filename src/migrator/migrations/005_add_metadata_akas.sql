CREATE TABLE public.imdb_metadata_akas (
	imdb_id varchar(16) NOT NULL,
	ordering integer NULL,
	localized_title varchar(8000) NULL,
	region varchar(10) NULL,
	"language" varchar(100) NULL,
	types varchar(200) NULL,
	attributes varchar(200) NULL,
	is_original_title boolean NULL,
    CONSTRAINT fk_imdb_metadata_akas_imdb_id
        FOREIGN KEY (imdb_id)
            REFERENCES public.imdb_metadata(imdb_id)
            ON DELETE CASCADE
);
