# Torrentio

- [torrentio-addon](addon) - the Stremio addon which will query scraped entries and return Stremio stream results.

## Self-hosted quickstart
```
docker-compose up -d
```
Then open your browser to `127.0.0.1:7000`

If you'd like to enable crawling of RealDebridManager's shared hashlists which will massively boost your database cached entries, 
enter a readonly github personal access token in 'env/producer.env' as the 'GithubSettings__PAT=<token_here>' value.

You can scale the number of consumers, by changing the consumer deploy replica count in the compose file on line 87. This is currently set to 3.
If you'd like to adjust the number of concurrent processed ingestions per consumer, thats the job concurrency setting within 'env/consumer.env'.