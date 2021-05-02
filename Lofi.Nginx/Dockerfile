FROM nginx:alpine
EXPOSE 80
VOLUME [ "/var/www/lofi/media" ]

COPY --from=lofi/app /app/public/ /var/www/lofi/

ENTRYPOINT [ "nginx", "-g", "daemon off;" ]
