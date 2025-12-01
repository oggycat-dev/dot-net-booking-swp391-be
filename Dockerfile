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

# Install EF Core tools for migrations
RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy published app
COPY --from=build /app/out .

# Copy migration files
COPY --from=build /app/src/CleanArchitectureTemplate.Infrastructure/Migrations ./Migrations

# Copy entrypoint script
COPY docker-entrypoint.sh /app/docker-entrypoint.sh
RUN chmod +x /app/docker-entrypoint.sh

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["/app/docker-entrypoint.sh"]