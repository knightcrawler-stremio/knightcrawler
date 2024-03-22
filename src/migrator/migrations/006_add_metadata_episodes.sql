CREATE TABLE public.imdb_metadata_episodes (
	episode_id varchar(16) NOT NULL,
	parent_id varchar(16) NOT NULL,
	season varchar(10) NOT NULL,
    episode varchar(10) NOT NULL,
    CONSTRAINT fk_imdb_metadata_episodes_parent_id
        FOREIGN KEY (parent_id)
            REFERENCES public.imdb_metadata(imdb_id)
            ON DELETE CASCADE
);
