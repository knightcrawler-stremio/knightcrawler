#!/bin/sh
export WAIT_HOSTS="${POSTGRES_HOST}:${POSTGRES_PORT}"
export WAIT_BEFORE=3
export WAIT_TIMEOUT=15
./wait && \
pgmigrate apply -d "postgres://${POSTGRES_USER}:${POSTGRES_PASSWORD}@${POSTGRES_HOST}:${POSTGRES_PORT}/${POSTGRES_DB}"
