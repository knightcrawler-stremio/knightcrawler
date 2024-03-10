FROM ghcr.io/peterldowns/pgmigrate:latest
USER root
RUN addgroup -S migrator && adduser -S -G migrator migrator
USER migrator
WORKDIR /tmp
COPY --chmod=0777 . .
ENTRYPOINT [ "./entrypoint.sh" ]
