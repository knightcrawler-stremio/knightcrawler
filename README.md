# Torrentio self-hosted

- [Torrentio self-hosted](#torrentio-self-hosted)
  - [Self-hosted quickstart](#self-hosted-quickstart)
  - [Raspberry Pi users](#raspberry-pi-users)

## Self-hosted quickstart

Start by cloning the repository. Everything is ready to go immediately for local use.

Simply open a terminal and run the command:

```
docker-compose up -d
```

Then open your browser to [http://127.0.0.1:7000](http://127.0.0.1:7000)

## Raspberry Pi users

You will need to use a different version of MongoDB. In the `docker-compose.yml` file you will need to change the following:

```
services:
  mongodb:
    restart: unless-stopped
    image: docker.io/bitnami/mongodb:7.0 # <--------- we are changing this line
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/bitnami/mongodb
```

to now read:

```
services:
  mongodb:
    restart: unless-stopped
    image: mongo:6.0 # <--------- we have changed this line
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/bitnami/mongodb
```
