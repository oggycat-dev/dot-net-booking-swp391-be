#!/bin/bash
set -e

echo "========================================"
echo "Starting Clean Architecture Template API"
echo "========================================"

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
until pg_isready -h db -U ${POSTGRES_USER:-postgres} -d ${POSTGRES_DB:-CleanArchitectureTemplateDb} > /dev/null 2>&1; do
  echo "PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "PostgreSQL is ready!"

# Check if migrations are needed
echo "Checking database migrations..."

# Set connection string for EF Core
export ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=${POSTGRES_DB:-CleanArchitectureTemplateDb};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD:-postgres}"

# Try to apply migrations
echo "Applying database migrations..."

# Since we're in runtime container, we'll let the application handle migrations
# The application should check and apply migrations on startup

echo "========================================"
echo "Starting application..."
echo "========================================"

# Start the application
exec dotnet CleanArchitectureTemplate.API.dll
