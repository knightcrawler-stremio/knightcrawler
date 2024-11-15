FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG TARGETARCH
WORKDIR /src
COPY shared/ shared/
COPY debrid-collector/ debrid-collector/
WORKDIR /src/debrid-collector/
RUN dotnet restore -a $TARGETARCH
RUN dotnet publish -c Release --no-restore -o /src/out -a $TARGETARCH


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19

WORKDIR /app

ENV PYTHONUNBUFFERED=1

RUN apk add --update --no-cache python3=~3.11 py3-pip && ln -sf python3 /usr/bin/python

COPY --from=build /src/out .

RUN rm -rf /app/python && mkdir -p /app/python

RUN pip3 install -r /app/requirements.txt -t /app/python

RUN addgroup -S debrid && adduser -S -G debrid debrid
USER debrid
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
  CMD pgrep -f dotnet || exit 1
ENV PYTHONNET_PYDLL=/usr/lib/libpython3.11.so.1.0
ENTRYPOINT ["dotnet", "DebridCollector.dll"]
