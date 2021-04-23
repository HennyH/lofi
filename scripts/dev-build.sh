#!/bin/sh
set -e

# build all the containers, in dev lofi/app is included in
# the compose file because it will be running the sirv dev
# server to enable live-reload and such.
docker-compose -f docker-compose.development.yml build $@
