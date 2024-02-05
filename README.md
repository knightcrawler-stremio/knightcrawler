# Knight Crawler
<img src="https://i.ibb.co/hYJPLdP/logo-only.png" alt="isolated" width="100"/>

A self-hosted Stremio addon for streaming torrents via a debrid service.

## Contents

> [!CAUTION] 
> Until we reach `v1.0.0`, please consider releases as alpha.

> [!IMPORTANT]
> The latest change renames the project and requires a [small migration](#selfhostio-to-knightcrawler-migration).
- [Knight Crawler](#knight-crawler)
  - [Contents](#contents)
  - [Overview](#overview)
  - [Using](#using)
    - [Download Docker and Docker Compose v2](#download-docker-and-docker-compose-v2)
    - [Environment Setup](#environment-setup)
      - [Optional Configuration Changes](#optional-configuration-changes)
    - [DebridMediaManager setup (optional)](#debridmediamanager-setup-optional)
    - [Run the project](#run-the-project)
    - [Monitoring with Grafana and Prometheus (Optional)](#monitoring-with-grafana-and-prometheus-optional)
      - [Accessing RabbitMQ Management](#accessing-rabbitmq-management)
      - [Using Grafana and Prometheus](#using-grafana-and-prometheus)
  - [Importing external dumps](#importing-external-dumps)
    - [Import data into database](#import-data-into-database)
    - [INSERT INTO ingested\_torrents](#insert-into-ingested_torrents)
  - [Selfhostio to KnightCrawler Migration](#selfhostio-to-knightcrawler-migration)
  - [To-do](#to-do)


## Overview

Stremio is a media player. On it's own it will not allow you to watch anything. This addon at it's core does the following:

1. It will search the internet and collect information about movies and tv show torrents, then store it in a database.
2. It will then allow you to click on the movie or tv show you desire in Stremio and play it with no further effort.

## Using

The project is shipped as an all-in-one solution. The initial configuration is designed for hosting only on your local network. If you want it to be accessible from outside of your local network, please see [not yet available]()

### Download Docker and Docker Compose v2

Download and install [Docker Compose](https://docs.docker.com/compose/install/), bundled with [Docker Desktop](https://docs.docker.com/desktop/) or, if using Linux, you can install [Docker Engine](https://docs.docker.com/engine/install/) and the [Docker Compose Plugin.](https://docs.docker.com/compose/install/linux/)

### Environment Setup

Before running the project, you need to set up the environment variables. Copy the `.env.example` file to `.env`:

```sh
cd deployment/docker
cp .env.example .env
```

Then set any of the values you wouldd like to customize.

#### Optional Configuration Changes

> [!WARNING]
> These values should be tested and tuned for your specific machine.

By default, Knight Crawler is configured to be *relatively* conservative in its resource usage. If running on a decent machine (16GB RAM, i5+ or equivalent), you can increase some settings to increase consumer throughput. This is especially helpful if you have a large backlog from [importing databases](#importing-external-dumps).

In your `.env` file, under the `# Consumer` section increase `CONSUMER_REPLICAS` from `3` to `15`.
You can also increase `JOB_CONCURRENCY` from `5` to `10`.

### DebridMediaManager setup (optional)

There are some optional steps you should take to maximise the number of movies/tv shows we can find. 

We can search DebridMediaManager hash lists which are hosted on GitHub. This allows us to add hundreds of thousands of movies and tv shows, but it requires a Personal Access Token to be generated. The software only needs read access and only for public respositories. To generate one, please follow these steps:

1. Navigate to GitHub settings -> Developer Settings -> Personal access tokens -> Fine-grained tokens (click [here](https://github.com/settings/tokens?type=beta) for a direct link)
2. Press `Generate new token`
3. Fill out the form (example data below):
   ```
    Token name:
        KnightCrawler
    Expiration:
        90 days
    Description:
        <blank>
    Respository access
        (checked) Public Repositories (read-only) 
   ```
4. Click `Generate token`
5. Take the new token and add it to the bottom of the [.env](deployment/docker/.env) file
   ```
   GithubSettings__PAT=<YOUR TOKEN HERE>
   ```


### Run the project

Open a terminal in the project directory and run the command:

```sh
cd deployment/docker
docker compose up -d
```

It will take a while to find and add the torrents to the database. During initial testing, in one hour it's estimated that around 200,000 torrents were located and added to the queue to be processed. For best results, you should leave everything running for a few hours.

To add the addon to Stremio, open a web browser and navigate to: [http://127.0.0.1:7000](http://127.0.0.1:7000)

### Monitoring with Grafana and Prometheus (Optional)

To enhance your monitoring capabilities, you can use Grafana and Prometheus in addition to RabbitMQ's built-in management interface. This allows you to visualize and analyze RabbitMQ metrics with more flexibility. With postgres-exporter service, you can also monitor Postgres metrics.

#### Accessing RabbitMQ Management


You can still monitor RabbitMQ by accessing its management interface at [http://127.0.0.1:15672/](http://127.0.0.1:15672/). Use the provided credentials to log in and explore RabbitMQ's monitoring features (the default username and password are `guest`).

#### Using Grafana and Prometheus

Here's how to set up and use Grafana and Prometheus for monitoring RabbitMQ:

1. **Start Grafana and Prometheus**: Run the following command to start both Grafana and Prometheus:

   ```sh
   cd deployment/docker
   docker compose -f docker-compose-metrics.yml up -d
   ```

   - Grafana will be available at [http://127.0.0.1:3000](http://127.0.0.1:3000).
   - Prometheus will be available at [http://127.0.0.1:9090](http://127.0.0.1:9090).

   - The default admin user for Grafana is `admin`, and the password is `admin_password`.

2. **Import Grafana Dashboard**: Import the RabbitMQ monitoring dashboard into Grafana:

   - You can use the following dashboard from Grafana's official library: [RabbitMQ Overview Dashboard](https://grafana.com/grafana/dashboards/10991-rabbitmq-overview/).

   - You can alse use the following dashboard [PostgreSQL Database](https://grafana.com/grafana/dashboards/9628-postgresql-database/) to monitor Postgres metrics.

   The Prometheus data source is already configured in Grafana, you just have to select it when importing the dashboard.


Now, you can use these dashboards to monitor RabbitMQ and Postgres metrics.

> [!NOTE]
>  If you encounter issues with missing or unavailable data in Grafana, please ensure on [Prometheus's target page](http://127.0.0.1:9090/targets) that the RabbitMQ target is up and running.


## Importing external dumps

A brief record of the steps required to import external data, in this case the rarbg dump which can be found on RD:

### Import data into database


Using [pgloader](https://pgloader.readthedocs.io/en/latest/ref/sqlite.html) we can import other databases into Knight Crawler.

For example, if you had a sql database called `rarbg_db.sqlite` stored in `/tmp/` you would create a file called `db.load` containing the following:

```
load database
     from sqlite:///tmp/rarbg_db.sqlite
     into postgresql://postgres:postgres@<docker-ip>/knightcrawler

with include drop, create tables, create indexes, reset sequences

  set work_mem to '16MB', maintenance_work_mem to '512 MB';
```

> [!TIP]
> Your `docker-ip` can be found using the following command:
> `docker network inspect knightcrawler-network | grep knightcrawler-postgres -A 4`

Then run `pgloader db.load` to create a new `items` table.

### INSERT INTO ingested_torrents


> [!NOTE]
> This is specific to this example external database, other databases may/will have different column names and the sql command will require tweaking

> [!IMPORTANT]
> The `processed` field should be `false` so that the consumers will properly process it.

Once the `items` table is available in the postgres database, put all the tv/movie items into the `ingested_torrents` table using `psql`.

This can be done by running the following command:

```
docker exec -it knightcrawler-postgres-1 psql -d knightcrawler -c "
INSERT INTO ingested_torrents (name, source, category, info_hash, size, seeders, leechers, imdb, processed, \"createdAt\", \"updatedAt\")
SELECT title, 'RARBG', cat, hash, size, NULL, NULL, imdb, false, current_timestamp, current_timestamp
FROM items where cat='tv' OR cat='movies';"
```

## Selfhostio to KnightCrawler Migration

With the renaming of the project, you will have to change your database name in order to keep your existing data.

**With your existing stack still running**, run:
```
docker exec -it torrentio-selfhostio-postgres-1 psql -c "
SELECT pg_terminate_backend(pid) FROM pg_stat_activity 
WHERE pid <> pg_backend_pid() AND datname = 'selfhostio'; 
ALTER DATABASE selfhostio RENAME TO knightcrawler;"
```
Make sure your postgres container is named `torrentio-selfhostio-postgres-1`, otherwise, adjust accordingly.

This command should return: `ALTER DATABASE`. This means your database is now renamed. You can now pull the new changes if you haven't already and run `docker compose up -d`.

## To-do

- [ ] Add a section on external access
- [ ] Add a troubleshooting section