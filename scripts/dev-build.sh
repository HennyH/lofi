#!/bin/sh
set -e

[ -z "$MONEROD_DATA_DIR" ] && { echo "ERROR: The environment variable MONEROD_DATA_DIR must be configured"; exit 1; }

# build all the containers, in dev lofi/app is included in
# the compose file because it will be running the sirv dev
# server to enable live-reload and such.
docker-compose -f docker-compose.development.yml build $@
