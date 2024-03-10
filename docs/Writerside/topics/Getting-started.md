# Getting started

Knight Crawler is provided as an all-in-one solution. This means we include all the necessary software you need to get started
out of the box.

## Before you start

Make sure that you have:

- A place to host Knight Crawler
- [Docker](https://docs.docker.com/get-docker/) and [Compose](https://docs.docker.com/compose/install/) installed
- A [GitHub](https://github.com/) account _(optional)_


## Download the files

Installing Knight Crawler is as simple as downloading a copy of the [deployment directory](https://github.com/Gabisonfire/knightcrawler/tree/master/deployment/docker).

A basic installation requires only two files:
- <path>deployment/docker/.env.example</path>
- <path>deployment/docker/docker-compose.yaml</path>.

For this guide I will be placing them in a directory on my home drive <path>~/knightcrawler</path>.

Rename the <path>.env.example</path> file to be <path>.env</path>

```
~/
└── knightcrawler/
    ├── .env
    └── docker-compose.yaml
```

## Initial configuration

Below are a few recommended configuration changes.

Open the <path>.env</path> file in your favourite editor.

> If you are using an external database, configure it in the <path>.env</path> file. Don't forget to disable the ones
> included in the <path>docker-compose.yaml</path>.

### Database credentials

It is strongly recommended that you change the credentials for the databases included with Knight Crawler. This is best done
before running Knight Crawler for the first time. It is much harder to change the passwords once the services have been started
for the first time.

```Bash
POSTGRES_PASSWORD=postgres
...
MONGODB_PASSWORD=mongo
...
RABBITMQ_PASSWORD=guest
```

Here's a few options on generating a secure password:

```Bash
# Linux
tr -cd '[:alnum:]' < /dev/urandom | fold -w 64 | head -n 1
# Or you could use openssl
openssl rand -hex 32
```
```Python
# Python
import secrets

print(secrets.token_hex(32))
```

### Your time zone

```Bash
TZ=London/Europe
```

A list of time zones can be found on [Wikipedia](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones)

### Consumers

```Bash
JOB_CONCURRENCY=5
...
MAX_CONNECTIONS_PER_TORRENT=10
...
CONSUMER_REPLICAS=3
```

These are totally subjective to your machine and network capacity. The above default is pretty minimal and will work on
most machines.

`JOB_CONCURRENCY` is how many films and tv shows the consumers should process at once. As this affects every consumer
this will likely cause exponential
strain on your system. It's probably best to leave this at 5, but you can try experimenting with it if you wish.

`MAX_CONNECTIONS_PER_TORRENT` is how many peers the consumer will attempt to connect to when it is trying to collect
metadata.
Increasing this value can speed up processing, but you will eventually reach a point where more connections are being
made than
your router can handle. This will then cause a cascading fail where your internet stops working. If you are going to
increase this value
then try increasing it by 10 at a time.

> Increasing this value increases the max connections for every parallel job, for every consumer. For example
> with the default values above this means that Knight Crawler will be on average making `(5 x 3) x 10 = 150`
> connections at any one time.
>
{style="warning"}

`CONSUMER_REPLICAS` is how many consumers should be initially started. This is best kept below 10 as GitHub rate limit
how fast we can access a list of torrent trackers. You can increase or decrease the number of consumers whilst the
service is running by running the command `docker compose up -d --scale consumer=<number>`. This value is best increased by 5 at a time.
Repeat this process until you have reached the desired level of consumers.

### GitHub personal access token

This step is optional but strongly recommended. [Debrid Media Manager](https://debridmediamanager.com/start) is a media library manager
for Debrid services. When a user of this service chooses to export/share their library publicly it is saved to a public GitHub repository.
This is, essentially, a repository containing a vast amount of ready to go films and tv shows. Knight Crawler comes with the ability to
read these exported lists, but it requires a GitHub account to make it work.

Knight Crawler needs a personal access token with read-only access to public repositories. This means we can not access any private
repositories you have.

1. Navigate to GitHub settings ([GitHub token settings](https://github.com/settings/tokens?type=beta)):
    - Navigate to `GitHub settings`.
    - Click on `Developer Settings`.
    - Select `Personal access tokens`.
    - Choose `Fine-grained tokens`.

2. Press `Generate new token`.

3. Fill out the form with the following information:
   ```Generic
   Token name:
       KnightCrawler
   Expiration:
       90 days
   Description:
       <blank>
   Respository access:
       (checked) Public Repositories (read-only) 
   ```

4. Click `Generate token`.

5. Take the new token and add it to the bottom of the <path>.env</path> file:
    ```Bash
    # Producer
    GITHUB_PAT=<YOUR TOKEN HERE>
    ```

## Start Knight Crawler

To start Knight Crawler use the following command:

```Bash
docker compose up -d
```

Then we can follow the logs to watch it start:

```Bash
docker compose logs -f --since 1m
```

> Knight Crawler will only be accessible on the machine you run it on, to make it accessible from other machines navigate to [External access](External-access.md).
>
{style="note"}

To stop following the logs press <shortcut>Ctrl+C</shortcut> at any time.

The Knight Crawler configuration page should now be accessible in your web browser at [http://localhost:7000](http://localhost:7000)

## Start more consumers

If you wish to speed up the processing of the films and tv shows that Knight Crawler finds, then you'll likely want to 
increase the number of consumers.

The below command can be used to both increase or decrease the number of running consumers. Gradually increase the number
until you encounter any issues and then decrease until stable.

```Bash
docker compose up -d --scale consumer=<number>
```

## Stop Knight Crawler

Knight Crawler can be stopped with the following command:

```Bash
docker compose down
```
