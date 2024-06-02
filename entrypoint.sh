#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Function to check if SQL Server is up and running
is_sql_server_running() {
  /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourPassword123 -Q "SELECT 1" &> /dev/null
}

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
while ! is_sql_server_running; do
  echo "SQL Server is still starting up..."
  sleep 5
done

echo "SQL Server is up and running."

# Run the initialization script
echo "Running the initialization script..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourPassword123 -i /init-db/init-db.sql

# Keep the container running
wait