FROM docker:stable

# Install Docker Compose
RUN apk add --no-cache docker-compose

# Set the working directory
WORKDIR /app

# Copy your Docker Compose configuration files into the container
COPY docker-compose.yml /app/docker-compose.yml

# Set the entrypoint to run Docker Compose up
ENTRYPOINT ["docker-compose"]
CMD ["up"]