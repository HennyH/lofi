#!/bin/sh
set -e

[ -z "$PG_DATA_DIR" ] && { echo "ERROR: The environment variable PG_DATA_DIR must be configured"; exit 1; }

npm run dev:stop api

docker-compose -f docker-compose.development.yml run -- api dotnet ef --msbuildprojectextensionspath obj/container $@

npm run dev:start api
