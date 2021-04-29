#!/bin/sh
set -e

echo $@

[ -z "$PG_DATA_DIR" ] && { echo "ERROR: The environment variable PG_DATA_DIR must be configured"; exit 1; }
[ -z "$MONEROD_DATA_DIR" ] && { echo "ERROR: The environment variable MONEROD_DATA_DIR must be configured"; exit 1; }

# start up all the development containers
docker-compose -f docker-compose.development.yml up $@
