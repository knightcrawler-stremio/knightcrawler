FROM docker.io/python:3.11-slim

WORKDIR /app

RUN pip install --no-cache-dir poetry
RUN poetry config virtualenvs.create false

COPY . /app
RUN poetry install --no-dev

CMD ["python", "/app/torrent_ingestor/main.py"]
