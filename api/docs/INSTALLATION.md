# Installation Guide

This document provides step-by-step instructions to set up the GymTracker application.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Node.js and npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Backend

1. Clone the repository:
    ```sh
    git clone https://github.com/cosmindevelops/fitness-tracker.git
    cd fitness-tracker
    ```

2. Navigate to the `GymTracker.API` project and restore dependencies:
    ```sh
    cd src/GymTracker.API
    dotnet restore
    ```

3. Update the connection string in `appsettings.json` to point to your SQL Server instance.

4. Apply migrations and seed the database:
    ```sh
    dotnet ef database update
    ```

5. Run the backend project:
    ```sh
    dotnet run
    ```

## Frontend

1. Navigate to the Angular project and install dependencies:
    ```sh
    cd frontend
    npm install
    ```

2. Run the Angular development server:
    ```sh
    ng serve
    ```

3. Open your browser and navigate to `http://localhost:4200`.

## Additional Configuration

- Ensure your SQL Server is running and accessible.
- Modify any additional settings as required in the `appsettings.json` for the backend and the `environment.ts` for the frontend.

## Troubleshooting

- If you encounter issues with database migrations, ensure your connection string is correct and the SQL Server instance is accessible.
- For frontend issues, ensure you have the correct version of Node.js and Angular CLI installed.

