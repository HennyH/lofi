FROM node:alpine AS builder
EXPOSE 80

# install node dependencies
WORKDIR /app
COPY package.json .
RUN npm install

# build the app
COPY . .
RUN npm run build

# remove non development dependencies
RUN npm prune

# by default start serving the app
CMD ["npm", "run", "start", "--", "--port", "80", "--host", "127.0.0.1"]
