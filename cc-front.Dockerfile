# syntax=docker/dockerfile:1
FROM node:20-alpine as angular
WORKDIR /ng-app
COPY src/Frontend/package*.json ./
RUN npm ci --legacy-peer-deps
COPY ./src/Frontend/ ./
RUN npm run build

FROM nginx:alpine
COPY ./src/Frontend/nginx.conf /etc/nginx/conf.d/
RUN rm /etc/nginx/conf.d/default.conf
RUN mv /etc/nginx/conf.d/nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=angular /ng-app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 80
