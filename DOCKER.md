# Docker Setup Guide

## Prerequisites
- Docker Desktop installed
- Docker Compose installed

## Quick Start

### 1. Configure Environment Variables
Copy the example environment file:
```bash
cp docker.env .env
```

Edit `.env` file with your configuration (especially change passwords in production!).

### 2. Build and Run
```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode (background)
docker-compose up -d --build
```

### 3. Access the Application
- **API Swagger**: http://localhost:5001
- **PostgreSQL**: localhost:5433 (mapped to avoid conflict with local PostgreSQL)

### 4. View Logs
```bash
# View all logs
docker-compose logs -f

# View API logs only
docker-compose logs -f api

# View database logs only
docker-compose logs -f db
```

## Docker Commands

### Stop Services
```bash
docker-compose stop
```

### Stop and Remove Containers
```bash
docker-compose down
```

### Stop and Remove Containers + Volumes (WARNING: This deletes the database!)
```bash
docker-compose down -v
```

### Restart Services
```bash
docker-compose restart
```

### Rebuild Without Cache
```bash
docker-compose build --no-cache
docker-compose up
```

### Check Running Containers
```bash
docker-compose ps
```

### Execute Commands in Container
```bash
# Access API container bash
docker-compose exec api bash

# Access database with psql
docker-compose exec db psql -U postgres -d CleanArchitectureTemplateDb
```

## Database Migrations

Migrations are automatically applied when the container starts. The application checks and applies pending migrations on startup.

### Manual Migration (if needed)
```bash
# Access the API container
docker-compose exec api bash

# Check migration status (if EF tools are available)
dotnet ef migrations list --project /app/CleanArchitectureTemplate.Infrastructure.dll

# Apply migrations manually
dotnet ef database update --project /app/CleanArchitectureTemplate.Infrastructure.dll
```

## Troubleshooting

### Port Conflicts
If ports 5001, 5002, or 5433 are already in use, edit `docker-compose.yml` to use different ports:
```yaml
ports:
  - "8080:80"  # Change 5001 to 8080
  - "5434:5432"  # Change 5433 to 5434
```

### Database Connection Issues
1. Check if database is healthy:
```bash
docker-compose ps
```

2. View database logs:
```bash
docker-compose logs db
```

3. Test database connection:
```bash
docker-compose exec db pg_isready -U postgres
```

### Application Not Starting
1. Check API logs:
```bash
docker-compose logs api
```

2. Ensure database is healthy before API starts:
```bash
docker-compose up db
# Wait for "database system is ready to accept connections"
docker-compose up api
```

### Reset Everything
```bash
# Stop and remove everything
docker-compose down -v

# Remove images
docker-compose down --rmi all

# Rebuild from scratch
docker-compose up --build
```

## Production Deployment

### 1. Create Production Environment File
```bash
cp docker.env .env.production
```

### 2. Update Production Values
Edit `.env.production`:
- Change all passwords
- Use strong JWT secret key
- Set `ASPNETCORE_ENVIRONMENT=Production`
- Update CORS origins
- Configure proper logging levels

### 3. Use Production Compose File
Create `docker-compose.prod.yml`:
```yaml
version: '3.8'
services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:80"
      - "443:443"
    env_file:
      - .env.production
    restart: always
    # Add SSL certificates volume
    volumes:
      - ./certs:/https:ro
```

### 4. Deploy
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## Health Checks

The application includes health checks:

### API Health Check
```bash
curl http://localhost:5001/health
```

### Database Health Check
```bash
docker-compose exec db pg_isready -U postgres
```

## Backup and Restore

### Backup Database
```bash
docker-compose exec db pg_dump -U postgres CleanArchitectureTemplateDb > backup.sql
```

### Restore Database
```bash
docker-compose exec -T db psql -U postgres CleanArchitectureTemplateDb < backup.sql
```

## Network Configuration

The application uses a custom bridge network `clean-architecture-network` for service communication.

Services communicate using container names:
- API connects to database using hostname: `db`
- Database is accessible externally on port `5433`

## Volume Management

### View Volumes
```bash
docker volume ls
```

### Inspect Volume
```bash
docker volume inspect dot-net-clean-architecture-template_postgres_data
```

### Backup Volume
```bash
docker run --rm -v dot-net-clean-architecture-template_postgres_data:/data -v $(pwd):/backup alpine tar czf /backup/postgres_backup.tar.gz /data
```

### Restore Volume
```bash
docker run --rm -v dot-net-clean-architecture-template_postgres_data:/data -v $(pwd):/backup alpine sh -c "cd /data && tar xzf /backup/postgres_backup.tar.gz --strip 1"
```
