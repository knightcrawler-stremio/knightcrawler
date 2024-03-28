import asyncio
import contextlib
from datetime import datetime, timedelta
from os import environ

import logger
import structlog
from pydantic import BaseModel
from redis import asyncio as redis
from sqlalchemy import Boolean, Column, DateTime, Integer, String, create_engine
from sqlalchemy.exc import IntegrityError
from sqlalchemy.orm import Session, declarative_base, sessionmaker

logger.init()
log = structlog.getLogger(__name__)

Base = declarative_base()


class TorrentAddedEvent(BaseModel):
    info_hash: str
    title: str
    imdb: str
    size: int
    indexer: str
    category: str
    season: int | None = None
    episode: int | None = None


class IngestedTorrent(Base):
    __tablename__ = "ingested_torrents"
    id = Column(Integer, primary_key=True, autoincrement=True)
    name = Column(String)
    source = Column(String, default="Annatar Pubsub")
    category = Column(String)
    info_hash = Column(String)
    size = Column(Integer, default=0)
    seeders = Column(Integer, default=0)
    leechers = Column(Integer, default=0)
    imdb = Column(String, nullable=False)
    processed = Column(Boolean, default=False)
    createdAt = Column(DateTime, default=datetime.utcnow)
    updatedAt = Column(DateTime, default=datetime.utcnow)


async def write_to_postgres(
    session: sessionmaker[Session],
    queue: asyncio.Queue[TorrentAddedEvent],
) -> None:
    while True:
        sess = session()
        try:
            event: TorrentAddedEvent = await queue.get()
            with contextlib.suppress(IntegrityError):
                sess.add(
                    IngestedTorrent(
                        name=event.title,
                        info_hash=event.info_hash,
                        imdb=event.imdb,
                        size=event.size,
                        category=event.category,
                        updatedAt=datetime.utcnow(),
                    )
                )
                sess.commit()
                log.info("saved torrent", torrent=event.info_hash)
            queue.task_done()
        except asyncio.CancelledError:
            return
        except Exception as e:
            log.error("error saving torrent", exc_info=e)
            await asyncio.sleep(1)
        finally:
            sess.close()


def run_redis_subscriber(
    redis_url: str, channel_name: str, queue: asyncio.Queue[TorrentAddedEvent]
) -> None:
    asyncio.run(redis_subscriber(redis_url, channel_name, queue))


def start_queue_processor(SessionLocal: sessionmaker[Session], queue: asyncio.Queue):
    asyncio.run(run_queue_processors(SessionLocal, queue, 10))


async def redis_subscriber(
    redis_url: str, channel_name: str, queue: asyncio.Queue[TorrentAddedEvent]
) -> None:
    db = redis.from_url(redis_url)
    pubsub = db.pubsub()
    await pubsub.subscribe(channel_name)
    try:
        async for message in pubsub.listen():
            if message["type"] == "message":
                log.debug("got message", message=message)
                event_data = TorrentAddedEvent.model_validate_json(message["data"].decode())
                # only process the event if we haven't seen it in the last 24 hours
                # this is to prevent processing the same event multiple times
                # to allow multiple consumers since redis pubsub is a broadcast
                if await db.set(
                    name=f"kc:torrents:seen:{event_data.info_hash}",
                    value=1,
                    nx=True,
                    ex=timedelta(days=1),
                ):
                    await queue.put(event_data)
    finally:
        await pubsub.unsubscribe(channel_name)
        await db.aclose()


async def main():
    REDIS_URL = environ.get("REDIS_URL", "redis://localhost")
    if not REDIS_URL:
        raise ValueError("REDIS_URL env var is required.")

    # Database connection settings
    DATABASE_URL = environ.get("POSTGRES_URL")
    if not DATABASE_URL:
        raise ValueError("POSTGRES_URL env var is required.")
    engine = create_engine(
        DATABASE_URL, pool_size=100, max_overflow=0, pool_timeout=5, pool_recycle=3600
    )
    SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

    Base.metadata.create_all(bind=engine, checkfirst=True)

    queue = asyncio.Queue[TorrentAddedEvent](maxsize=100)

    await asyncio.wait(
        [
            asyncio.create_task(redis_subscriber(REDIS_URL, "events:v1:torrent:added", queue)),
            asyncio.create_task(run_queue_processors(SessionLocal, queue, 10)),
        ]
    )


async def run_queue_processors(
    session: sessionmaker[Session], queue: asyncio.Queue, n_processors: int
):
    tasks = [asyncio.create_task(write_to_postgres(session, queue)) for _ in range(n_processors)]
    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
