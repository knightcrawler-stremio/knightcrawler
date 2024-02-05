FROM node:lts-buster-slim as builder

RUN apt-get update && \
    apt-get install -y git && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Cache dependencies
COPY ./consumer/package*.json ./consumer/
COPY ./common/package*.json ./common/
COPY package*.json ./
RUN npm install

# Copy project and build
COPY ./consumer/ ./consumer/
COPY ./common/ ./common/
RUN npm -w consumer run build

# --- Runtime Stage ---
FROM node:lts-buster-slim

WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app ./
RUN npm -w consumer prune --omit=dev

EXPOSE 7001

ENTRYPOINT [ "node", "consumer/dist/index.cjs" ]