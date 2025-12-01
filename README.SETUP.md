# 🚀 Quick Setup Guide for Frontend Team

## Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running
- Git installed

## 📥 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/oggycat-dev/dot-net-clean-architecture-template.git
cd dot-net-clean-architecture-template
```

### 2. Get Environment File from Backend Team
Contact your backend developer to get the `.env` file and place it in the root directory:
```
dot-net-clean-architecture-template/
├── .env                    # ← Place the .env file here
├── docker-compose.yml
├── Dockerfile
└── ...
```

### 3. Start the Application
```bash
docker-compose up -d
```

That's it! 🎉 The application will:
- ✅ Start PostgreSQL database
- ✅ Wait for database to be ready
- ✅ Apply all database migrations automatically
- ✅ Initialize admin account
- ✅ Start the API server

### 4. Access Swagger API Documentation
Open your browser and go to:
- **API Documentation**: http://localhost:5001

## 🔐 Default Admin Account
- **Email**: `admin@cleanarchitecture.com`
- **Password**: `Admin@12345`

Use this account to login and get JWT token for testing protected endpoints.

## 📡 API Endpoints

### Public Endpoints (No Authentication)
- `POST /api/Auth/login` - Login and get JWT token

### Protected Endpoints (Require JWT Token)
#### User API
- `GET /api/Users` - Get all users (Admin only)
- `GET /api/Users/{id}` - Get user by ID (Admin only)
- `POST /api/Users` - Create user (Admin only)
- `PUT /api/Users/{id}` - Update user (Admin only)
- `DELETE /api/Users/{id}` - Delete user (Admin only)

#### CMS API
- All endpoints under `/api/cms/*` require Admin role

## 🔑 How to Use JWT Token

### 1. Login to Get Token
```bash
curl -X POST http://localhost:5001/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@cleanarchitecture.com",
    "password": "Admin@12345"
  }'
```

Response:
```json
{
  "statusCode": 200,
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "email": "admin@cleanarchitecture.com",
    "role": "Admin"
  },
  "timestamp": "2025-11-12T15:30:00.000Z"
}
```

### 2. Use Token in Requests
Add the token to Authorization header:
```bash
curl -X GET http://localhost:5001/api/Users \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### In Swagger UI:
1. Click the **Authorize** button (lock icon) at the top right
2. Enter: `Bearer YOUR_TOKEN_HERE`
3. Click **Authorize**
4. Now you can test all protected endpoints

## 📋 Useful Docker Commands

### View Logs
```bash
# All logs
docker-compose logs -f

# API logs only
docker-compose logs -f api

# Database logs only
docker-compose logs -f db
```

### Stop Services
```bash
docker-compose stop
```

### Restart Services
```bash
docker-compose restart
```

### Stop and Remove Containers
```bash
docker-compose down
```

### Rebuild and Restart (after code changes)
```bash
docker-compose down
docker-compose up -d --build
```

## 🐛 Troubleshooting

### Port Already in Use
If you see errors about ports 5001, 5002, or 5433 already in use:

1. Check what's using the port:
   ```bash
   # macOS/Linux
   lsof -i :5001
   
   # Windows
   netstat -ano | findstr :5001
   ```

2. Either stop that process or change ports in `docker-compose.yml`

### Database Connection Failed
1. Check if database container is running:
   ```bash
   docker-compose ps
   ```

2. Check database logs:
   ```bash
   docker-compose logs db
   ```

3. Restart services:
   ```bash
   docker-compose restart
   ```

### API Not Starting
1. Check API logs:
   ```bash
   docker-compose logs api
   ```

2. Make sure database is healthy before API starts:
   ```bash
   docker-compose up db
   # Wait for "database system is ready to accept connections"
   docker-compose up api
   ```

### "Migration Failed" Error
This usually means database connection issue. Check your `.env` file:
- `POSTGRES_DB` should match `App__Database__DatabaseName`
- `POSTGRES_USER` should match database username in connection string
- `POSTGRES_PASSWORD` should match database password

### Reset Everything
If something goes wrong, reset everything:
```bash
# Stop and remove containers, volumes, networks
docker-compose down -v

# Start fresh
docker-compose up -d --build
```

## 📞 Need Help?
Contact your backend developer if you encounter any issues!

## 📚 Additional Resources
- [Docker Documentation](DOCKER.md) - Detailed Docker configuration and commands
- [API Architecture](ARCHITECTURE.md) - Project architecture and structure
- [Environment Variables](env.example) - All available environment variables
