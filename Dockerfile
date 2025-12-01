# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and project file
COPY *.sln .
COPY global.json .
COPY src/CleanArchitectureTemplate.API/*.csproj ./src/CleanArchitectureTemplate.API/
COPY src/CleanArchitectureTemplate.Application/*.csproj ./src/CleanArchitectureTemplate.Application/
COPY src/CleanArchitectureTemplate.Domain/*.csproj ./src/CleanArchitectureTemplate.Domain/
COPY src/CleanArchitectureTemplate.Infrastructure/*.csproj ./src/CleanArchitectureTemplate.Infrastructure/

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the source code
COPY src/. ./src/

# Build the application
RUN dotnet publish src/CleanArchitectureTemplate.API/CleanArchitectureTemplate.API.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install PostgreSQL client for pg_isready and wait-for-it script
RUN apt-get update && apt-get install -y postgresql-client netcat-traditional && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/out .

# Create wwwroot directory for file uploads
RUN mkdir -p /app/wwwroot && chmod 755 /app/wwwroot

EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Start the application directly
CMD ["dotnet", "CleanArchitectureTemplate.API.dll"]