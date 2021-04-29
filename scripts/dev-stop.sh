#!/bin/sh
set -e

docker-compose -f docker-compose.development.yml stop -- $@
