#!/bin/sh
set -e

# start up all the development containers
docker-compose -f docker-compose.development.yml up $@
