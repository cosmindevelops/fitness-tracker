# Stage 1: Compile and Build angular codebase
FROM node:latest as build

WORKDIR /usr/src/app

COPY ./ /usr/src/app/

RUN npm install
RUN npm run build

# Stage 2: Serve app with nginx server
FROM nginx:latest

# Copy the build output to the NGINX html directory
COPY --from=build /usr/src/app/dist/frontend/browser /usr/share/nginx/html

# Copy custom NGINX configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
