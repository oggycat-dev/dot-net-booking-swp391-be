#!/bin/bash
set -e

echo "========================================"
echo "Starting FPT Booking System API"
echo "========================================"

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
until pg_isready -h db -U ${POSTGRES_USER:-postgres} -d ${POSTGRES_DB:-CleanArchitectureTemplateDb} > /dev/null 2>&1; do
  echo "PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "PostgreSQL is ready!"

# Set connection string for EF Core
export ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=${POSTGRES_DB:-CleanArchitectureTemplateDb};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD:-postgres}"

echo "========================================"
echo "Starting application..."
echo "Application will apply migrations automatically"
echo "========================================"

# Start the application
exec dotnet CleanArchitectureTemplate.API.dll
