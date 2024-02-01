FROM node:lts-buster-slim

# RUN apk update && apk upgrade && \
#     apk add --no-cache git curl

RUN apt-get update && \
    apt-get install -y curl git && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /home/node/app

COPY package*.json ./
RUN npm ci --only-production
COPY . .
RUN chmod a+x ./check-ip.sh

CMD [ "node", "--no-warnings=ExperimentalWarning", "index.js" ]