# --- Build Stage ---
FROM node:lts-alpine AS builder

RUN apk update && apk upgrade && \
    apk add --no-cache git

WORKDIR /app

COPY ./addon/package*.json ./
RUN npm install
COPY ./addon/ .
RUN npm run build

# Create the utility package

WORKDIR /utility

COPY ./utility/package*.json ./
RUN npm install
COPY ./utility/ .
# RUN npm run build

# --- Runtime Stage ---
FROM node:lts-alpine

# Install pm2
RUN npm install pm2 -g

WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app ./
COPY --from=builder /utility /utility
RUN npm prune --omit=dev

EXPOSE 7000

ENTRYPOINT [ "pm2-runtime", "start", "ecosystem.config.cjs"]