FROM nginx:alpine
EXPOSE 80
VOLUME [ "/var/www/lofi/media" ]

COPY --from=lofi/app /app/public/ /var/www/lofi/
COPY nginx.conf /etc/nginx/nginx.conf

ENTRYPOINT [ "nginx", "-g", "daemon off;" ]

