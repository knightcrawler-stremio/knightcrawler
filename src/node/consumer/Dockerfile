FROM node:lts-buster-slim as builder

RUN apt-get update && \
    apt-get install -y git && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# --- Runtime Stage ---
FROM node:lts-buster-slim

WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app ./
RUN npm prune --omit=dev

# CIS-DI-0001
RUN useradd -d /home/consumer -m -s /bin/bash consumer
# CIS-DI-0008
RUN chmod u-s /usr/bin/wall /usr/bin/expiry /usr/bin/chage /usr/bin/passwd /usr/bin/gpasswd /usr/bin/chsh /usr/bin/newgrp /usr/bin/chfn /sbin/unix_chkpwd /bin/su /bin/mount /bin/umount /usr/bin/chage /usr/bin/expiry /usr/bin/wall
USER consumer

EXPOSE 7001
ENTRYPOINT [ "node", "dist/index.cjs" ]
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
    CMD pgrep -x node > /dev/null || exit 1
