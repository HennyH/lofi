#!/bin/sh
set -e

nginx $@

while inotifywait -q -e modify /etc/nginx/nginx.conf; do
    nginx -s reload
done
