# Clean Architecture Template

A .NET 8 Web API project following Clean Architecture principles with CQRS pattern, Domain-Driven Design (DDD), and comprehensive configuration management.

## 🏗️ Architecture Overview

This template implements Clean Architecture with the following layers:

- **Domain Layer**: Contains enterprise business rules, entities, enums, and value objects
- **Application Layer**: Contains application business rules, CQRS commands/queries, DTOs, and interfaces
- **Infrastructure Layer**: Contains external concerns, data access, and service implementations
- **API Layer**: Contains web API controllers, middleware, and configurations

## 🎯 Key Features

- ✅ **Clean Architecture** - Separation of concerns with dependency inversion
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation using MediatR
- ✅ **Domain-Driven Design** - Rich domain models with business logic
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Dependency Injection** - IoC container configuration
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Input validation
- ✅ **Entity Framework Core** - ORM with SQL Server support
- ✅ **Swagger/OpenAPI** - API documentation
- ✅ **Serilog** - Structured logging
- ✅ **Global Exception Handling** - Centralized error handling
- ✅ **Environment Configuration** - .env file support
- ✅ **CORS Configuration** - Cross-origin resource sharing
- ✅ **Soft Delete** - Audit trail with soft deletion
- ✅ **Pagination** - Built-in pagination support

## 📁 Project Structure

```
CleanArchitectureTemplate/
├── src/
│   ├── CleanArchitectureTemplate.Domain/          # Domain Layer
│   │   ├── Commons/                               # Base entities and extensions
│   │   ├── Entities/                              # Domain entities
│   │   ├── Enums/                                 # Enumeration types
│   │   └── ValueObjects/                          # Value objects
│   │
│   ├── CleanArchitectureTemplate.Application/     # Application Layer
│   │   ├── Common/                                # Shared application components
│   │   │   ├── Behaviors/                         # MediatR behaviors
│   │   │   ├── DTOs/                              # Data transfer objects
│   │   │   ├── Exceptions/                        # Custom exceptions
│   │   │   ├── Interfaces/                        # Application interfaces
│   │   │   ├── Mappings/                          # AutoMapper profiles
│   │   │   └── Models/                            # Application models
│   │   └── Features/                              # Application features
│   │       └── Users/                             # User feature
│   │           ├── Commands/                      # CQRS Commands
│   │           └── Queries/                       # CQRS Queries
│   │
│   ├── CleanArchitectureTemplate.Infrastructure/  # Infrastructure Layer
│   │   ├── Persistence/                           # Database context
│   │   ├── Repositories/                          # Data access implementations
│   │   ├── Services/                              # External service implementations
│   │   └── Configurations/                        # Infrastructure configurations
│   │
│   └── CleanArchitectureTemplate.API/            # API Layer
│       ├── Controllers/                            # API controllers
│       ├── Configurations/                        # API configurations
│       ├── Middlewares/                           # Custom middleware
│       ├── Filters/                               # Action filters
│       ├── Extensions/                            # Extension methods
│       └── Injection/                            # Dependency injection
│
├── env.example                                    # Environment variables template
└── CleanArchitectureTemplate.sln                 # Solution file
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or later / Visual Studio Code
- SQL Server (LocalDB or higher)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CleanArchitectureTemplate
   ```

2. **Set up environment variables**
   ```bash
   cp env.example .env
   # Edit .env file with your configuration
   ```

3. **Navigate to the API project**
   ```bash
   cd src/CleanArchitectureTemplate.API
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Apply database migrations**
   ```bash
   dotnet ef database update --project ../CleanArchitectureTemplate.Infrastructure --startup-project . --context ApplicationDbContext
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

   Or with hot reload:
   ```bash
   dotnet watch run
   ```

7. **Access the API**
   - Swagger UI: `https://localhost:5001` or `http://localhost:5000`
   - API Base URL: `https://localhost:5001/api`

## 🔧 Configuration

### Environment Variables

The application supports configuration through `.env` files. Copy `env.example` to `.env` and customize:

```env
# Database Configuration
ConnectionStrings__DefaultConnection="Server=(localdb)\\mssqllocaldb;Database=CleanArchitectureTemplateDb;Trusted_Connection=true;MultipleActiveResultSets=true"

# Application Configuration
App__Environment="Development"
App__Name="Clean Architecture Template"
App__Version="1.0.0"

# CORS Configuration
Cors__AllowedOrigins__0="http://localhost:3000"
Cors__AllowedOrigins__1="https://localhost:3000"

# JWT Configuration
JWT__SecretKey="your-super-secret-key-here-must-be-at-least-32-characters-long"
JWT__Issuer="CleanArchitectureTemplate"
JWT__Audience="CleanArchitectureTemplate"
JWT__ExpirationMinutes=60
```

### Database Configuration

The application uses Entity Framework Core with SQL Server. Update the connection string in your configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchitectureTemplateDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## 🏛️ Architecture Patterns

### Clean Architecture

The solution follows Clean Architecture principles:

- **Dependency Rule**: Dependencies point inward
- **Domain Layer**: Contains business entities and rules
- **Application Layer**: Contains use cases and application logic
- **Infrastructure Layer**: Contains external concerns
- **API Layer**: Contains web-specific concerns

### CQRS Pattern

Commands and Queries are separated:

- **Commands**: Modify state (Create, Update, Delete)
- **Queries**: Read data (Get, List, Search)

Example:
```csharp
// Command
public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// Query
public class GetUsersQuery : PaginatedQuery<UserDto>
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
```

### Domain-Driven Design

Rich domain models with business logic:

```csharp
public class User : BaseEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    
    // Domain methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public string FullName => $"{FirstName} {LastName}".Trim();
}
```

## 📝 API Documentation

The API includes comprehensive Swagger documentation available at `/swagger` endpoint.

### Example Endpoints

- `GET /api/users` - Get paginated list of users
- `POST /api/users` - Create a new user

## 🧪 Development

### Adding New Features

1. **Create Domain Entity** (if needed)
   ```csharp
   public class Product : BaseEntity
   {
       public string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

2. **Create Application Commands/Queries**
   ```csharp
   public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
   {
       public string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

3. **Create API Controller**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class ProductsController : ControllerBase
   {
       [HttpPost]
       public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductCommand command)
       {
           var result = await _mediator.Send(command);
           return Ok(result);
       }
   }
   ```

4. **Update Database Context**
   ```csharp
   public DbSet<Product> Products => Set<Product>();
   ```

5. **Create Migration**
   ```bash
   dotnet ef migrations add AddProduct --project ../CleanArchitectureTemplate.Infrastructure --startup-project .
   ```

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName --project ../CleanArchitectureTemplate.Infrastructure --startup-project .
```

Apply migrations:
```bash
dotnet ef database update --project ../CleanArchitectureTemplate.Infrastructure --startup-project .
```

## 🛠️ Technologies Used

- **.NET 8.0** - Framework
- **Entity Framework Core** - ORM
- **MediatR** - CQRS implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Validation
- **Serilog** - Logging
- **Swagger/OpenAPI** - API documentation
- **SQL Server** - Database
- **DotNetEnv** - Environment configuration

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📞 Support

For support and questions, please open an issue in the repository.
