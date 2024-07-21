#!/usr/bin/env pwsh

# Launches the "psql" command in the terminal with a connection to the
# docker-compose postgres db instance

docker exec -ti db-db-1 psql -U postgres # -d otp