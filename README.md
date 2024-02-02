# Selfhostio

A self-hosted Stremio addon for streaming torrents via a debrid service.

## Overview

Stremio is a media player. On it's own it will not allow you to watch anything. This addon at it's core does the following:

1. It will search the internet and collect information about movies and tv show torrents, then store it in a database.
2. It will then allow you to click on the movie or tv show you desire in Stremio and play it with no further effort.

## Using

```
docker-compose up -d
```
Then open your browser to `127.0.0.1:7000`

If you'd like to enable crawling of RealDebridManager's shared hashlists which will massively boost your database cached entries, 
enter a readonly github personal access token in 'env/producer.env' as the 'GithubSettings__PAT=<token_here>' value.

You can scale the number of consumers, by changing the consumer deploy replica count in the compose file on line 87. This is currently set to 3.
If you'd like to adjust the number of concurrent processed ingestions per consumer, thats the job concurrency setting within 'env/consumer.env'.