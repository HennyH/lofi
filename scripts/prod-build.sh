#!/bin/sh
set -e

# build the lofi/app image which the lofi/nginx image will
# reference, I don't know how to encode such a dependency
# in the compose file so we have to do it here.
docker build -t lofi/app Lofi.App

# now that lofi/app has been built we can build all the images
# in the compose file.
docker-compose -f docker-compose.yml build $@
