#!/bin/bash

docker build -t ippexdeploymentscr.azurecr.io/dave/stremio-addon-jackett:latest . --platform linux/amd64
docker push ippexdeploymentscr.azurecr.io/dave/stremio-addon-jackett:latest