# syntax=docker/dockerfile:1
FROM node:20-alpine as angular
WORKDIR /ng-app
COPY src/Frontend/package*.json ./
RUN npm install -g npm@10.5.0
RUN npm ci --legacy-peer-deps
COPY ./src/Frontend/ ./
RUN npm run build

FROM nginx:alpine
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=angular /ng-app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 8080
