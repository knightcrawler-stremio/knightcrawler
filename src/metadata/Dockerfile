FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG TARGETARCH
WORKDIR /App
COPY . ./
RUN dotnet restore -a $TARGETARCH
RUN dotnet publish -c Release --no-restore -o out -a $TARGETARCH

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

ENV ASPNETCORE_URLS=http://+:5050

WORKDIR /App
COPY --from=build /App/out .
RUN addgroup -S metadata && adduser -S -G metadata metadata
USER metadata
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
  CMD pgrep -f dotnet || exit 1
ENTRYPOINT ["dotnet", "Metadata.dll"]
