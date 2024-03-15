
# syntax=docker/dockerfile:1
FROM node:20-alpine as angular
WORKDIR /ng-app
COPY package*.json .
RUN npm ci --legacy-peer-deps
COPY . .
RUN npm run build

FROM nginx:alpine
COPY ./nginx.conf /etc/nginx/conf.d/
RUN cd /etc/nginx/conf.d/
RUN ls

RUN rm /etc/nginx/conf.d/default.conf
RUN ls

RUN mv /etc/nginx/conf.d/nginx.conf /etc/nginx/conf.d/default.conf
RUN ls

COPY --from=angular /ng-app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 4200
