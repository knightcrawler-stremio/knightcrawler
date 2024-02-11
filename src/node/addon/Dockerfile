# --- Build Stage ---
FROM node:lts-alpine AS builder

RUN apk update && apk upgrade && \
    apk add --no-cache git

WORKDIR /app

COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# --- Runtime Stage ---
FROM node:lts-alpine

# Install pm2
RUN npm install pm2 -g

WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app ./
RUN npm prune --omit=dev

# CIS-DI-0001
RUN addgroup -S addon && adduser -S -G addon addon
USER addon

EXPOSE 7000

ENTRYPOINT [ "pm2-runtime", "start", "ecosystem.config.cjs"]
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
    CMD pm2 ping || exit 1
