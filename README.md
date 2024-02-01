# Torrentio self-hosted

- [Torrentio self-hosted](#torrentio-self-hosted)
  - [Self-hosted quickstart](#self-hosted-quickstart)
  - [Raspberry Pi users](#raspberry-pi-users)
  - [FAQ](#faq)
    - [MongoDB not working](#mongodb-not-working)
      - [I have a Raspberry Pi](#i-have-a-raspberry-pi)
      - [I don't have a Raspberry Pi](#i-dont-have-a-raspberry-pi)

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
      - mongo-data:/bitnami/mongodb # <--------- we are changing this line
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
      - mongo-data:/data/db # <--------- we have changed this line
```


## FAQ

### MongoDB not working
<details>
<summary>You may need a different version of MongoDB</summary>

#### I have a Raspberry Pi

You need to switch to a `linux/arm64` compatible image. Please see [Raspberry Pi users](#raspberry-pi-users)

#### I don't have a Raspberry Pi

You may have an incompatible CPU. Try the following command:

`lscpu | grep avx` or `grep avx /proc/cpuinfo`

If you get a return value that contains `avx` or `avx2` such as: `flags : fpu ... sse sse2 ss syscall nx pdpe1gb rdtscp ... aes xsave avx avx2 hypervisor lahf_lm arat tsc_adjust xsaveopt` then your CPU is compatible and you should open an issue.

If you get nothing back from the above two commands, your CPU is incompatible with MongoDB newer than version 4.

Change the image to be:

```
services:
  mongodb:
    restart: unless-stopped
    image: docker.io/bitnami/mongodb:7.0 # <--------- we are changing this line
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/bitnami/mongodb # <--------- we are changing this line
```

to now read:

```
services:
  mongodb:
    restart: unless-stopped
    image: mongo:4.4.6 # <--------- we have changed this line
    ports:
      - "27017:27017"
    volumes:
      mongo-data:/data/db # <--------- we have changed this line
```

</details>
