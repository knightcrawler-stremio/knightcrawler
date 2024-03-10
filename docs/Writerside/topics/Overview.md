# Overview

<img alt="The image shows a Knight in silvery armour looking forwards." src="knight-crawler-logo.png" title="Knight Crawler logo" width="100"/>

Knight Crawler is a self-hosted [Stremio](https://www.stremio.com/) addon for streaming torrents via a [Debrid](Supported-Debrid-services.md "Click for a list of Debrid services we support") service.

We are active on [Discord](https://discord.gg/8fQdxay9z2) for both support and casual conversation.

> Knight Crawler is currently alpha software.
> 
> Users are responsible for ensuring their data is backed up regularly.
> 
> Please read the changelogs before updating to the latest version.
>
{style="warning"}

## What does Knight Crawler do?

Knight Crawler is an addon for [Stremio](https://www.stremio.com/). It began as a fork of the very popular
[Torrentio](https://github.com/TheBeastLT/torrentio-scraper) addon. Knight crawler essentially does the following:

1. It searches the internet for available films and tv shows.
2. It collects as much information as it can about each film and tv show it finds.
3. It then stores this information to a database for easy access.

When you choose on a film or tv show to watch on Stremio, a request will be sent to your installation of Knight Crawler.
Knight Crawler will query the database and return a list of all the copies it has stored in the database as Debrid links.
This enables playback to begin immediately for your chosen media.