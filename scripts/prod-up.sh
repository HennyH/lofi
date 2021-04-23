#!/bin/sh
set -e

# start up all the production containers
docker-compose -f docker-compose.yml up $@
