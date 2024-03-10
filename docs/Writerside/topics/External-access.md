# External access

This guide outlines how to use Knight Crawler on devices like your TV. While it's currently limited to the device of
installation, we can change that. With some extra effort, we'll show you how to make it accessible on other devices.
This limitation is set by Stremio, as [explained here](https://github.com/Stremio/stremio-features/issues/687#issuecomment-1890546094).

## What to keep in mind

Before we make Knight Crawler available outside your home network, we've got to talk about safety. No software is
perfect, including ours. Knight Crawler is built on lots of different parts, some made by other people. So, if we keep
it just for your home network, it's a bit safer. But if you want to use it over the internet, just know that keeping
your devices secure is up to you. We won't be responsible for any problems or lost data if you use Knight Crawler that way.

## Initial setup

To enable external access for Knight Crawler, whether it's within your home network or over the internet, you'll
need to follow these initial setup steps:

- Set up Caddy, a powerful and easy-to-use web server.
- Disable the open port in the Knight Crawler <path>docker-compose.yaml<path> file.


### Caddy

A basic Caddy configuration is included with Knight Crawler in the deployment directory.

<path>deployment/docker/optional-services/caddy</path>

```Generic
deployment/
└── docker/
    └── optional-services/
        └── caddy/
            ├── config/
            │   ├── snippets/
            │   │   └── cloudflare-replace-X-Forwarded-For
            │   └── Caddyfile
            ├── logs/
            └── docker-compose.yaml
```

ports:
  - "8080:8080"

By disabling the default port, Knight Crawler will only be accessible internally within your network, ensuring added security.

## Home network access

## Internet access

### Through a VPN

### On the public web

## Troubleshooting?

## Additional Resources?
