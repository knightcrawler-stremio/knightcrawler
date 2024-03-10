CREATE TABLE public.contents (
  "infoHash" character varying(64) NOT NULL,
  "fileIndex" integer NOT NULL,
  path character varying(512) NOT NULL,
  size bigint
);

ALTER TABLE public.contents
ADD CONSTRAINT contents_pkey
PRIMARY KEY ("infoHash", "fileIndex");

CREATE SEQUENCE public.files_id_seq;

CREATE TABLE public.files (
  id integer PRIMARY KEY NOT NULL DEFAULT nextval('files_id_seq'::regclass),
  "infoHash" character varying(64) NOT NULL,
  "fileIndex" integer,
  title character varying(512) NOT NULL,
  size bigint,
  "imdbId" character varying(32),
  "imdbSeason" integer,
  "imdbEpisode" integer,
  "kitsuId" integer,
  "kitsuEpisode" integer,
  "createdAt" timestamp with time zone NOT NULL,
  "updatedAt" timestamp with time zone NOT NULL
);

CREATE INDEX files_imdb_id_imdb_season_imdb_episode ON public.files USING btree ("imdbId", "imdbSeason", "imdbEpisode");

CREATE INDEX files_kitsu_id_kitsu_episode ON public.files USING btree ("kitsuId", "kitsuEpisode");

CREATE UNIQUE INDEX files_unique_file_constraint ON public.files USING btree ("infoHash", "fileIndex", "imdbId", "imdbSeason", "imdbEpisode", "kitsuId", "kitsuEpisode");

ALTER SEQUENCE public.files_id_seq OWNED BY public.files.id;

CREATE SEQUENCE public.ingested_pages_id_seq;

CREATE TABLE public.ingested_pages (
  id integer PRIMARY KEY NOT NULL DEFAULT nextval('ingested_pages_id_seq'::regclass),
  url character varying(512) UNIQUE NOT NULL,
  "createdAt" timestamp with time zone NOT NULL,
  "updatedAt" timestamp with time zone NOT NULL
);

ALTER SEQUENCE public.ingested_pages_id_seq OWNED BY public.ingested_pages.id;

CREATE SEQUENCE public.ingested_torrents_id_seq;

CREATE TABLE public.ingested_torrents (
  id integer PRIMARY KEY NOT NULL DEFAULT nextval('ingested_torrents_id_seq'::regclass),
  name character varying(512),
  source character varying(512),
  category character varying(32),
  info_hash character varying(64),
  size character varying(32),
  seeders integer,
  leechers integer,
  imdb character varying(32),
  processed boolean DEFAULT false,
  "createdAt" timestamp with time zone NOT NULL,
  "updatedAt" timestamp with time zone NOT NULL
);

CREATE UNIQUE INDEX ingested_torrent_unique_source_info_hash_constraint ON public.ingested_torrents USING btree (source, info_hash);

ALTER SEQUENCE public.ingested_torrents_id_seq OWNED BY public.ingested_torrents.id;

CREATE TABLE public.providers (
  name character varying(32) PRIMARY KEY NOT NULL,
  "lastScraped" timestamp with time zone,
  "lastScrapedId" character varying(128)
);

CREATE TABLE public.skip_torrents (
  "infoHash" character varying(64) PRIMARY KEY NOT NULL
);

CREATE SEQUENCE public.subtitles_id_seq;

CREATE TABLE public.subtitles (
  id integer PRIMARY KEY NOT NULL DEFAULT nextval('subtitles_id_seq'::regclass),
  "infoHash" character varying(64) NOT NULL,
  "fileIndex" integer NOT NULL,
  "fileId" bigint,
  title character varying(512) NOT NULL
);

CREATE INDEX subtitles_file_id ON public.subtitles USING btree ("fileId");

CREATE UNIQUE INDEX subtitles_unique_subtitle_constraint ON public.subtitles USING btree ("infoHash", "fileIndex", "fileId");

ALTER SEQUENCE public.subtitles_id_seq OWNED BY public.subtitles.id;

CREATE TABLE public.torrents (
  "infoHash" character varying(64) PRIMARY KEY NOT NULL,
  provider character varying(32) NOT NULL,
  "torrentId" character varying(512),
  title character varying(512) NOT NULL,
  size bigint,
  type character varying(16) NOT NULL,
  "uploadDate" timestamp with time zone NOT NULL,
  seeders smallint,
  trackers character varying(8000),
  languages character varying(4096),
  resolution character varying(16),
  reviewed boolean NOT NULL DEFAULT false,
  opened boolean NOT NULL DEFAULT false,
  "createdAt" timestamp with time zone NOT NULL,
  "updatedAt" timestamp with time zone NOT NULL
);
