# Zentry

A modular monolith application built with .NET 9, designed to be a scalable foundation for productivity applications.

## Architecture

Zentry follows a **modular monolith** architecture pattern:

- **Zentry.Api**: Minimal API with Swagger, Serilog, and global middleware
- **Zentry.Domain**: Core business entities, value objects, and domain rules
- **Zentry.Application**: DTOs, validation, pagination, and result patterns
- **Zentry.Infrastructure**: EF Core DbContext, configurations, and data access
- **Zentry.Modules.Tasks**: Task management module (fully implemented)
- **Zentry.Modules.Notes**: Notes module (skeleton implementation)
- **Zentry.Tests**: Integration tests using xUnit and FluentAssertions

## Features

### Tasks Module
- ✅ CRUD operations for tasks
- ✅ Task completion toggle
- ✅ Pagination and filtering
- ✅ Search functionality
- ✅ Due date management
- ✅ Input validation with FluentValidation

### Notes Module
- ✅ Basic health check endpoint
- 🔄 Full CRUD operations (planned)

### Infrastructure
- ✅ PostgreSQL with SQLite fallback
- ✅ EF Core with automatic migrations
- ✅ Global exception handling
- ✅ Request logging with Serilog
- ✅ CORS support for development
- ✅ Health check endpoints

## Quick Start

### Prerequisites
- .NET 9 SDK
- Docker and Docker Compose (optional)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Zentry
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run --project Zentry.Api
   ```

5. **Access the API**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health

### Database Setup

The application uses SQLite by default for development. The database file (`zentry.db`) will be created automatically on first run.

#### Using PostgreSQL (Production)

1. **Set connection string**
   ```bash
   export ConnectionStrings__Default="Host=localhost;Database=zentry;Username=postgres;Password=yourpassword"
   ```

2. **Run migrations**
   ```bash
   dotnet ef migrations add InitialCreate -p Zentry.Infrastructure -s Zentry.Api
   dotnet ef database update -p Zentry.Infrastructure -s Zentry.Api
   ```

### Docker Setup

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

2. **Access the application**
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080/swagger
   - PostgreSQL: localhost:5432

3. **Stop the services**
   ```bash
   docker-compose down
   ```

## API Endpoints

### Tasks

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/tasks` | Get paginated list of tasks |
| GET | `/api/v1/tasks/{id}` | Get task by ID |
| POST | `/api/v1/tasks` | Create a new task |
| PUT | `/api/v1/tasks/{id}` | Update an existing task |
| PATCH | `/api/v1/tasks/{id}/toggle` | Toggle task completion status |
| DELETE | `/api/v1/tasks/{id}` | Delete a task |

### Query Parameters (GET /api/v1/tasks)
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20)
- `isDone`: Filter by completion status (optional)
- `search`: Search in title and description (optional)

### Notes
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/notes/health` | Health check for Notes module |

## Testing

### Run Tests
```bash
dotnet test
```

### Test Coverage
The project includes integration tests for the Tasks module:
- Task creation and persistence
- Task listing with filters and search
- Task completion toggle

## Development

### Adding New Modules

1. **Create module project**
   ```bash
   dotnet new classlib -n Zentry.Modules.{ModuleName}
   ```

2. **Add project references**
   - Add reference to `Zentry.Domain`
   - Add reference to `Zentry.Application`

3. **Create module structure**
   ```
   Zentry.Modules.{ModuleName}/
   ├── Entities/
   ├── DTOs/
   ├── Validators/
   ├── Mappings/
   ├── Endpoints/
   └── Services/
   ```

4. **Add to API project**
   - Add project reference to API
   - Map endpoints in `Program.cs`

### Code Style

- **Nullable reference types**: Enabled
- **Async/await**: Required for I/O operations
- **Manual mapping**: No AutoMapper
- **FluentValidation**: For input validation
- **Result pattern**: For operation outcomes

### Commit Messages

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
feat(tasks): add task completion toggle endpoint
fix(api): resolve validation error handling
docs: update README with quickstart guide
```

## CI/CD

The project includes GitHub Actions workflow for:
- Build verification
- Test execution
- Code coverage reporting

## Technology Stack

- **.NET 9**: Latest .NET version
- **EF Core**: Object-relational mapping
- **PostgreSQL**: Primary database
- **SQLite**: Development fallback
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **xUnit**: Testing framework
- **FluentAssertions**: Test assertions
- **Docker**: Containerization

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Roadmap

### Phase 1 (Current)
- ✅ Tasks module with full CRUD
- ✅ Notes module skeleton
- ✅ Basic infrastructure setup

### Phase 2 (Planned)
- 🔄 Complete Notes module
- 🔄 Habits module
- 🔄 Tags system
- 🔄 File attachments
- 🔄 User authentication

### Phase 3 (Future)
- 🔄 Real-time notifications
- 🔄 Mobile API
- 🔄 Advanced search
- 🔄 Data export/import
- 🔄 Plugin system
