# Run Knight Crawler

To run Knight Crawler you need two files, both can be found in the [deployment/docker](https://github.com/Gabisonfire/knightcrawler/tree/master/deployment/docker)
directory on GitHub:

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

## Configuration

Before we start the services, we need to change a few things in the <path>.env</path> file.

> If you are using an external database, configure it in the <path>.env</path> file. Don't forget to disable the ones
> included in the <path>docker-compose.yaml</path>.

### Your time zone.

```Bash
TZ=London/Europe
```

> A list of time zones can be found on [Wikipedia](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones)

### Consumers

```Bash
JOB_CONCURRENCY=5
...
MAX_CONNECTIONS_PER_TORRENT=10
...
CONSUMER_REPLICAS=3
```

These are totally subjective to your machine and network capacity. The above default is pretty minimal and will work on most machines.

`JOB_CONCURRENCY` is how many films and tv shows the consumers should process at once. As this affects every consumer this will likely cause exponential
 strain on your system. It's probably best to leave this at 5, but you can try experimenting with it if you wish.

`MAX_CONNECTIONS_PER_TORRENT` is how many peers the consumer will attempt to connect to when it is trying to collect metadata.
Increasing this value can speed up processing, but you will eventually reach a point where more connections are being made than
your router can handle. This will then cause a cascading fail where your internet stops working. If you are going to increase this value
then try increasing it by 10 at a time.

> Increasing this value increases the max connections for every parallel job for every consumer. For example
> with the default values above this means that Knight Crawler will be on average making `(5 x 3) x 10 = 150` connections at any one time.
>
{style="warning"}

`CONSUMER_REPLICAS` is how many consumers should be started. This is the ultimate decider in how fast you will be able to
add films and tv shows to your database. However, this is also going to be the most intensive service you will run.
The default of 3 is a reasonable starting amount. It will work on almost every system.