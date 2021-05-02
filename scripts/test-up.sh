#!/bin/sh
set -e

echo $@

[ -z "$TEST_PG_DATA_DIR" ] && { echo "ERROR: The environment variable TEST_PG_DATA_DIR must be configured"; exit 1; }
[ -z "$TEST_MONEROD_DATA_DIR" ] && { echo "ERROR: The environment variable TEST_MONEROD_DATA_DIR must be configured"; exit 1; }
[ -z "$TEST_MUSIC_DIRECTORY" ] && { echo "ERROR: The environment variable TEST_MUSIC_DIRECTORY must be configured"; exit 1; }

# start up all the development containers
docker-compose -f docker-compose.test.yml up $@
