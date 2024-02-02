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

EXPOSE 7001

ENTRYPOINT [ "pm2-runtime", "start", "ecosystem.config.cjs"]