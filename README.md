# Selfhostio

A self-hosted Stremio addon for streaming torrents via a debrid service.

## Contents

- [Selfhostio](#selfhostio)
  - [Contents](#contents)
  - [Overview](#overview)
  - [Using](#using)
    - [Initial setup (optional)](#initial-setup-optional)
    - [Run the project](#run-the-project)
    - [Monitoring with Grafana and Prometheus (Optional)](#monitoring-with-grafana-and-prometheus-optional)
  - [Importing external dumps](#importing-external-dumps)
    - [Import data into database](#import-data-into-database)
    - [INSERT INTO ingested\_torrents](#insert-into-ingested_torrents)
  - [To-do](#to-do)


## Overview

Stremio is a media player. On it's own it will not allow you to watch anything. This addon at it's core does the following:

1. It will search the internet and collect information about movies and tv show torrents, then store it in a database.
2. It will then allow you to click on the movie or tv show you desire in Stremio and play it with no further effort.

## Using

The project is shipped as an all-in-one solution. The initial configuration is designed for hosting only on your local network. If you want it to be accessible from outside of your local network, please see [not yet available]()

### Initial setup (optional)

After cloning the repository there are some steps you should take to maximise the number of movies/tv shows we can find. 

We can search DebridMediaManager hash lists which are hosted on GitHub. This allows us to add hundreds of thousands of movies and tv shows, but it requires a Personal Access Token to be generated. The software only needs read access and only for public respositories. To generate one, please follow these steps:

1. Navigate to GitHub settings -> Developer Settings -> Personal access tokens -> Fine-grained tokens (click [here](https://github.com/settings/tokens?type=beta) for a direct link)
2. Press `Generate new token`
3. Fill out the form (example data below):
   ```
    Token name:
        Selfhostio
    Expiration:
        90 days
    Description:
        <blank>
    Respository access
        (checked) Public Repositories (read-only) 
   ```
4. Click `Generate token`
5. Take the new token and add it to the bottom of the [env/producer.env](env/producer.env) file
   ```
   GithubSettings__PAT=<YOUR TOKEN HERE>
   ```

### Run the project

Open a terminal in the directory and run the command:

``` sh
docker compose up -d
```

It will take a while to find and add the torrents to the database. During initial testing, in one hour it's estimated that around 200,000 torrents were located and added to the queue to be processed. For best results, you should leave everything running for a few hours.

To add the addon to Stremio, open a web browser and navigate to: [http://127.0.0.1:7000](http://127.0.0.1:7000)

### Monitoring with Grafana and Prometheus (Optional)

To enhance your monitoring capabilities, you can use Grafana and Prometheus in addition to RabbitMQ's built-in management interface. This allows you to visualize and analyze RabbitMQ metrics with more flexibility.

#### Accessing RabbitMQ Management

You can still monitor RabbitMQ by accessing its management interface at [http://127.0.0.1:15672/](http://127.0.0.1:15672/). Use the provided credentials to log in and explore RabbitMQ's monitoring features (the default username and password are `guest`).

#### Using Grafana and Prometheus

Here's how to set up and use Grafana and Prometheus for monitoring RabbitMQ:

1. **Start Grafana and Prometheus**: Run the following command to start both Grafana and Prometheus:

   ```sh
   docker compose -f docker-compose-metrics.yml up -d
   ```

   - Grafana will be available at [http://127.0.0.1:3000](http://127.0.0.1:3000).
   - Prometheus will be available at [http://127.0.0.1:9090](http://127.0.0.1:9090).

   - The default admin user for Grafana is `admin`, and the password is `admin_password`.

2. **Import Grafana Dashboard**: Import the RabbitMQ monitoring dashboard into Grafana:

   - You can use the following dashboard from Grafana's official library: [RabbitMQ Overview Dashboard](https://grafana.com/grafana/dashboards/10991-rabbitmq-overview/).

   The Prometheus data source is already configured in Grafana, you just have to select it when importing the dashboard.


Now, you can use the imported dashboard to visualize RabbitMQ metrics collected by Prometheus.

Note: If you encounter issues with missing or unavailable data in Grafana, please ensure on [Prometheus's target page](http://127.0.0.1:9090/targets) that the RabbitMQ target is up and running.

## Importing external dumps

A brief record of the steps required to import external data, in this case the rarbg dump which can be found on RD:

### Import data into database

I created a file `db.load` as follows..

```
load database
     from sqlite:///tmp/rarbg_db.sqlite
     into postgresql://postgres:postgres@localhost/selfhostio

with include drop, create tables, create indexes, reset sequences

  set work_mem to '16MB', maintenance_work_mem to '512 MB';
```

And ran `pgloader db.load` to create a new `items` table.

### INSERT INTO ingested_torrents

Once the `items` table is available in the postgres database, put all the tv/movie items into the `ingested_torrents` table using:

```
INSERT INTO ingested_torrents (name, source, category, info_hash, size, seeders, leechers, imdb, processed, "createdAt", "updatedAt")
SELECT title, 'RARBG', cat, hash, size, NULL, NULL, imdb, false, current_timestamp, current_timestamp
FROM items where cat='tv' OR cat='movies';
```

## To-do

- [ ] Add a section on external access
- [ ] Add a troubleshooting section