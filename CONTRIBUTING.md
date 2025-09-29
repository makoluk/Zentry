# Contributing to Zentry

Thank you for your interest in contributing to Zentry! This document provides guidelines for contributing to the project.

## Development Setup

1. Clone the repository
2. Ensure you have .NET 9 SDK installed
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet build` to build the solution
5. Run `dotnet test` to run tests

## Code Style

This project uses the following conventions:

- **C# 9+ features**: Use modern C# features where appropriate
- **Nullable reference types**: Enabled throughout the project
- **Async/await**: Use async/await for all I/O operations
- **Cancellation tokens**: Include CancellationToken parameters in async methods
- **Manual mapping**: No AutoMapper - use manual mapping extensions
- **Result pattern**: Use Result<T> for operation outcomes
- **FluentValidation**: Use FluentValidation for input validation

## Commit Messages

We follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

### Format
```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### Types
- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation only changes
- `style`: Changes that do not affect the meaning of the code
- `refactor`: A code change that neither fixes a bug nor adds a feature
- `perf`: A code change that improves performance
- `test`: Adding missing tests or correcting existing tests
- `chore`: Changes to the build process or auxiliary tools

### Examples
```
feat(tasks): add task completion toggle endpoint
fix(api): resolve validation error handling
docs: update README with quickstart guide
refactor(domain): extract common entity base class
test(tasks): add integration tests for task CRUD operations
```

## Pull Request Process

1. Create a feature branch from `main`
2. Make your changes following the code style guidelines
3. Add tests for new functionality
4. Ensure all tests pass
5. Update documentation if needed
6. Submit a pull request with a clear description

## Testing

- Write unit tests for business logic
- Write integration tests for API endpoints
- Use FluentAssertions for test assertions
- Use in-memory database for testing

## Architecture Guidelines

This project follows a modular monolith architecture:

- **Domain**: Core business entities and rules
- **Application**: DTOs, validation, and application services
- **Infrastructure**: Data access and external services
- **Modules**: Feature-specific implementations (Tasks, Notes, etc.)
- **API**: Minimal API endpoints and middleware

### Adding New Modules

1. Create a new project: `Zentry.Modules.{ModuleName}`
2. Add the module project reference to the API project
3. Create entities, DTOs, validators, and endpoints
4. Add entity configurations to Infrastructure
5. Map endpoints in the API Program.cs
6. Add tests for the new module

## Database Migrations

When making changes to entities:

1. Add a new migration: `dotnet ef migrations add {MigrationName} -p Zentry.Infrastructure -s Zentry.Api`
2. Update the database: `dotnet ef database update -p Zentry.Infrastructure -s Zentry.Api`

## Questions?

If you have questions about contributing, please open an issue or start a discussion.
