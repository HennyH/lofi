#!/bin/sh
set -e

[ -z "$MONEROD_DATA_DIR" ] && { echo "ERROR: The environment variable MONEROD_DATA_DIR must be configured"; exit 1; }

# start up all the production containers
docker-compose -f docker-compose.yml up $@
