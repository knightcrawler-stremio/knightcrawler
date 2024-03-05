# Knight Crawler
<img src="https://i.ibb.co/fQxLJ1C/kc-final-e.png" alt="isolated" width="100"/>

A self-hosted Stremio addon for streaming torrents via a debrid service.

Join our [Discord](https://discord.gg/8fQdxay9z2)!

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
    - [Configure external access](#configure-external-access)
    - [Run the project](#run-the-project)
    - [Monitoring with Grafana and Prometheus (Optional)](#monitoring-with-grafana-and-prometheus-optional)
      - [Accessing RabbitMQ Management](#accessing-rabbitmq-management)
      - [Using Grafana and Prometheus](#using-grafana-and-prometheus)
  - [Importing external dumps](#importing-external-dumps)
    - [Importing data into PostgreSQL](#importing-data-into-postgresql)
      - [Using pgloader via docker](#using-pgloader-via-docker)
      - [Using native installation of pgloader](#using-native-installation-of-pgloader)
    - [Process the data we have imported](#process-the-data-we-have-imported)
    - [I imported the data without the `LIKE 'movies%%'` queries!](#i-imported-the-data-without-the-like-movies-queries)
  - [Selfhostio to KnightCrawler Migration](#selfhostio-to-knightcrawler-migration)
  - [To-do](#to-do)


## Overview

Stremio is a media player. On it's own it will not allow you to watch anything. This addon at it's core does the following:

1. It will search the internet and collect information about movies and tv show torrents, then store it in a database.
2. It will then allow you to click on the movie or tv show you desire in Stremio and play it with no further effort.

## Using

The project is shipped as an all-in-one solution. The initial configuration is designed for hosting only on your local network. If you want it to be accessible from outside of your local network, please see [Configure external access](#configure-external-access).

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
### Configure external access

What you will need:
1. Domain or subdomain that points toward your IP. You can use [DuckDNS](duckdns.org) for a free subdomain. [Installation instructions](http://www.duckdns.org/install.jsp) are provided to keep your IP updated.
2. Ports 80 and 443 opened on your router/gateway and forwarded to your Knightcrawler server. Refer to [PortForward.com](https://portforward.com/). Please note that this action may pose security vulnerabilities and potential damage for which Knightcrawler and its contributors cannot be held responsible.

### Run the project

> [!IMPORTANT]
> If you have configured external access, utilize the following commands:
> ```sh
>cd deployment/docker
>docker-compose -f docker-compose.yaml -f docker-compose-caddy.yaml up -d
>```

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

If you stumble upon some data, perhaps a database dump from rarbg, which is almost certainly not available on Real Debrid, and wished to utilise it, you might be interested in the following:

### Importing data into PostgreSQL

Using [pgloader](https://pgloader.readthedocs.io/en/latest/ref/sqlite.html) we can import other databases into Knight Crawler.

For example, if you *possessed* a sqlite database named `rarbg_db.sqlite` it's possible to then import this.

#### Using pgloader via docker

---

Make sure the file `rarbg_db.sqlite` is on the same server as PostgreSQL. Place it in any temp directory, it doesn't matter, we will delete the these files afterwards to free up storage.

For this example I'm placing my files in `~/tmp/load`

So after uploading the file, I have `~/tmp/load/rarbg_db.sqlite`. We need to create the file `~/tmp/load/db.load` which should contain the following:

```
load database
     from sqlite:///data/rarbg_db.sqlite
     into postgresql://postgres:postgres@postgres:5432/knightcrawler

with include drop, create tables, create indexes, reset sequences

  set work_mem to '16MB', maintenance_work_mem to '512 MB';
```

> [!NOTE]
> If you have changed the default password for PostgreSQL (RECOMMENDED) please change it above accordingly
> 
> `postgresql://postgres:<password here>@postgres:5432/knightcrawler`

Then run the following docker run command to import the database:

```
docker run --rm -it --network=knightcrawler-network -v "$(pwd)":/data dimitri/pgloader:latest pgloader /data/db.load
```

> [!IMPORTANT]
> The above line does not work on ARM devices (Raspberry Pi, etc). The image does not contain a build that supports ARM.
>
> I found [jahan9/pgloader](https://hub.docker.com/r/jahan9/pgloader) which supports ARM64 but I offer no guarantees that this is a safe image.
>
> USE RESPONSIBLY
>
> The command to utilise this image would be:
> ```
> docker run --rm -it --network=knightcrawler-network -v "$(pwd)":/data jahan9/pgloader:latest pgloader /data/db.load
> ```

#### Using native installation of pgloader

---

Similarly to above, make sure the file `rarbg_db.sqlite` is on the same server as PostgreSQL. Place it in any temp directory, it doesn't matter, we will delete the these files afterwards to free up storage.

For this example I'm placing my files in `~/tmp/load`

So after uploading the file, I have `~/tmp/load/rarbg_db.sqlite`. We need to create the file `~/tmp/load/db.load` which should contain the following:

```
load database
     from sqlite://~/tmp/load/rarbg_db.sqlite
     into postgresql://postgres:postgres@<docker-ip>/knightcrawler

with include drop, create tables, create indexes, reset sequences

  set work_mem to '16MB', maintenance_work_mem to '512 MB';
```

> [!NOTE]
> If you have changed the default password for PostgreSQL (RECOMMENDED) please change it above accordingly
> 
> `postgresql://postgres:<password here>@<docker-ip>/knightcrawler`

> [!TIP]
> Your `docker-ip` can be found using the following command:
> ```
> docker network inspect knightcrawler-network | grep knightcrawler-postgres -A 4
> ```

Then run `pgloader db.load`.

### Process the data we have imported

The data we have imported is not usable immediately. It is loaded into a new table called `items`. We need to move this data into the `ingested_torrents` table to be processed. The producer and atleast one consumer need to be running to process this data.

We are going to concatenate all of the different movie categories into one e.g. movies_uhd, movies_hd, movies_sd -> `movies`. This will give us a lot more data to work with.

The following command will process the tv shows:

```
docker exec -it knightcrawler-postgres-1 psql -U postgres -d knightcrawler -c "
INSERT INTO ingested_torrents (name, source, category, info_hash, size, seeders, leechers, imdb, processed, \"createdAt\", \"updatedAt\")
SELECT title, 'RARBG', 'tv', hash, size, NULL, NULL, imdb, false, current_timestamp, current_timestamp
FROM items where cat LIKE 'tv%%' ON CONFLICT DO NOTHING;"
```

This will do the movies:

```
docker exec -it knightcrawler-postgres-1 psql -U postgres -d knightcrawler -c "
INSERT INTO ingested_torrents (name, source, category, info_hash, size, seeders, leechers, imdb, processed, \"createdAt\", \"updatedAt\")
SELECT title, 'RARBG', 'movies', hash, size, NULL, NULL, imdb, false, current_timestamp, current_timestamp
FROM items where cat LIKE 'movies%%' ON CONFLICT DO NOTHING;"
```

Both should return a response that looks like:

```
INSERT 0 669475
```

As soon as both commands have been run, we can immediately drop the items table. The data inside has now been processed, and will just take up space.

```
docker exec -it knightcrawler-postgres-1 psql -U postgres -d knightcrawler -c "drop table items";
```


### I imported the data without the `LIKE 'movies%%'` queries!

If you've already imported the database using the old instructions, you will have imported a ton of new movies and tv shows, but the new queries will give you a lot more!

Don't worry, we can still correct this. We can retroactively update the categories by running the following commands

```
# Update movie categories
docker exec -it knightcrawler-postgres-1 psql -d knightcrawler -c "UPDATE ingested_torrents  SET category='movies', processed='f' WHERE category LIKE 'movies_%';"

# Update TV categories
docker exec -it knightcrawler-postgres-1 psql -d knightcrawler -c "UPDATE ingested_torrents  SET category='tv', processed='f' WHERE category LIKE 'tv_%';"
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

- [ ] Add a troubleshooting section
