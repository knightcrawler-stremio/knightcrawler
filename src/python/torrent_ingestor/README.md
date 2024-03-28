## Torrent Processor

This project subscribes to the Annatar Redis pubsub event `events:v1:torrent:added` and writes the results to `ingested_torrents` table.

Why? The Annatar event occurs when Annatar identifies a torrent from Jackett. This adds another source of torrents to the KC backed.

## Run

```bash
POSTGRES_URL=postgresql://USERNAME:PASSWORD@127.0.0.1/knightcrawler \
	REDIS_URL=redis://localhost \
	python torrent_ingestor/main.py
```

You can run multiple instances as it uses a SETNX to acquire a lock for each info hash.