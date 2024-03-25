CREATE TABLE public.imdb_metadata (
	imdb_id varchar(16) NOT NULL,
	category varchar(50) NULL,
	title varchar(1000) NULL,
	"year" varchar(10) NULL,
	adult boolean NULL,
	CONSTRAINT imdb_metadata_pk PRIMARY KEY (imdb_id)
);