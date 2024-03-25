FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG TARGETARCH
WORKDIR /src
COPY shared/ shared/
COPY producer/ producer/
WORKDIR /src/producer/src
RUN dotnet restore -a $TARGETARCH
RUN dotnet publish -c Release --no-restore -o /src/out -a $TARGETARCH


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

WORKDIR /app
COPY --from=build /src/out .
RUN addgroup -S producer && adduser -S -G producer producer
USER producer
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
  CMD pgrep -f dotnet || exit 1
ENTRYPOINT ["dotnet", "Producer.dll"]
