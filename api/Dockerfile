# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project files
COPY api/src/GymTracker.API/GymTracker.API.csproj GymTracker.API/
COPY api/src/GymTracker.Core/GymTracker.Core.csproj GymTracker.Core/
COPY api/src/GymTracker.Infrastructure/GymTracker.Infrastructure.csproj GymTracker.Infrastructure/

# Restore dependencies
RUN dotnet restore GymTracker.API/GymTracker.API.csproj

# Copy the rest of the files
COPY api/src/GymTracker.API/ GymTracker.API/
COPY api/src/GymTracker.Core/ GymTracker.Core/
COPY api/src/GymTracker.Infrastructure/ GymTracker.Infrastructure/

# Build the application
RUN dotnet build GymTracker.API/GymTracker.API.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish GymTracker.API/GymTracker.API.csproj -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install curl, sqlcmd, and mssql-tools (optional)
RUN apt-get update && \
    apt-get install -y curl apt-transport-https gnupg2 && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools unixodbc-dev && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Set the working directory
WORKDIR /app

# Copy the build output
COPY --from=build /app/publish .

# Copy the .env file from the src/GymTracker.API directory
COPY api/src/GymTracker.API/.env .env

# Expose the port the app runs on
EXPOSE 5000

# Run the application
CMD ["dotnet", "GymTracker.API.dll"]