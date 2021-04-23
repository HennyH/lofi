FROM alpine:latest
LABEL maintainer "hollingworth.henry@gmail.com"
EXPOSE 8000
VOLUME [ "/var/log/icecast" ]

# set the workdir to /home
WORKDIR /home

# install icecast and mailcap, this also creates
# a user:group icecast:icecast with the appropraite
# permissions
RUN apk add --update icecast mailcap \
    && rm -rf /var/cache/apk/*

COPY icecast.xml /etc/icecast.xml
CMD ["icecast", "-c", "/etc/icecast.xml"]
