# Selfhostio

A self-hosted Stremio addon for streaming torrents via a debrid service.

## Contents

- [Selfhostio](#selfhostio)
  - [Contents](#contents)
  - [Overview](#overview)
  - [Using](#using)
    - [Initial setup (optional)](#initial-setup-optional)
    - [Run the project](#run-the-project)
  - [To-do](#to-do)


## Overview

Stremio is a media player. On it's own it will not allow you to watch anything. This addon at it's core does the following:

1. It will search the internet and collect information about movies and tv show torrents, then store it in a database.
2. It will then allow you to click on the movie or tv show you desire in Stremio and play it with no further effort.

## Using

The project is shipped as an all-in-one solution. The initial configuration is designed for hosting only on your local network. If you want it to be accessible from outside of your local network, please see [not yet available]()

### Initial setup (optional)

After cloning the repository there are some steps you should take to maximise the number of movies/tv shows we can find. 

We can search DebridMediaManager hash lists which are hosted on GitHub. This allows us to add hundreds of thousands of movies and tv shows, but it requires a Personal Access Token to be generated. The software only needs read access and only for public respositories. To generate one, please follow these steps:

1. Navigate to GitHub settings -> Developer Settings -> Personal access tokens -> Fine-grained tokens (click [here](https://github.com/settings/tokens?type=beta) for a direct link)
2. Press `Generate new token`
3. Fill out the form (example data below):
   ```
    Token name:
        Selfhostio
    Expiration:
        90 days
    Description:
        <blank>
    Respository access
        (checked) Public Repositories (read-only) 
   ```
4. Click `Generate token`
5. Take the new token and add it to the bottom of the [env/producer.env](env/producer.env) file
   ```
   GithubSettings__PAT=<YOUR TOKEN HERE>
   ```

### Run the project

Open a terminal in the directory and run the command:

``` sh
docker compose up -d
```

It will take a while to find and add the torrents to the database. During initial testing, in one hour it's estimated that around 200,000 torrents were located and added to the queue to be processed. For best results, you should leave everything running for a few hours.

To add the addon to Stremio, open a web browser and navigate to: [http://127.0.0.1:7000](http://127.0.0.1:7000)


## To-do

- [ ] Add a section on external access
- [ ] Add a troubleshooting section