FROM docker.io/python:3.11-slim

WORKDIR /app

RUN pip install --no-cache-dir poetry
RUN poetry config virtualenvs.create false

COPY . /app
RUN poetry install --no-dev

RUN groupadd ingestor && useradd -g ingestor ingestor
USER ingestor

HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
  CMD pgrep -f python || exit 1
  
CMD ["python", "/app/torrent_ingestor/main.py"]
