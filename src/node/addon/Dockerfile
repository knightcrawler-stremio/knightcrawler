FROM node:16-alpine

RUN apk update && apk upgrade && \
    apk add --no-cache nodejs npm git curl && \
    rm -fr /var/cache/apk/*
    
WORKDIR /app

COPY package*.json .

RUN npm ci --only-production

COPY . .

EXPOSE 7000

CMD ["/usr/bin/node", "--insecure-http-parser", "/app/index.js" ]