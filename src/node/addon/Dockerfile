# --- Build Stage ---
FROM node:lts-alpine AS builder

RUN apk update && apk upgrade && \
    apk add --no-cache git

WORKDIR /app

# Cache dependencies
COPY ./addon/package*.json ./addon/
COPY ./common/package*.json ./common/
COPY package*.json ./
RUN npm install

# Copy project and build
COPY ./addon/ ./addon/
COPY ./common/ ./common/
RUN npm -w addon run build

# --- Runtime Stage ---
FROM node:lts-alpine

# Install pm2
RUN npm install pm2 -g

WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app ./
RUN npm prune --omit=dev

EXPOSE 7000

ENTRYPOINT [ "pm2-runtime", "start", "addon/ecosystem.config.cjs"]