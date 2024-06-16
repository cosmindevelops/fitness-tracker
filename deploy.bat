@echo off
echo Building Docker images...
docker-compose build

echo Logging in to Docker Hub...
docker login -u andrew999

echo Tagging Docker images...
docker tag fitness-tracker_backend:latest andrew999/fitness-tracker-backend:latest
docker tag fitness-tracker_frontend:latest andrew999/fitness-tracker-frontend:latest

echo Pushing Docker images to Docker Hub...
docker push andrew999/fitness-tracker-backend:latest
docker push andrew999/fitness-tracker-frontend:latest

echo Deployment complete!
pause