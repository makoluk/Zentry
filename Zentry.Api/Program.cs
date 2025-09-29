using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Zentry.Application.Interfaces;
using Zentry.Infrastructure.Data;
using Zentry.Api.Middleware;
using Zentry.Api.Models;
using Zentry.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Zentry API", Version = "v1" });
});

// Database configuration with fallback
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback to SQLite
    connectionString = "Data Source=zentry.db";
    builder.Services.AddDbContext<ZentryDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    // Use configured connection string (PostgreSQL in production)
    builder.Services.AddDbContext<ZentryDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Zentry.Application.Features.Tasks.Commands.CreateTask.CreateTaskCommand).Assembly);

// Add MediatR
builder.Services.AddMediatR(typeof(Zentry.Application.Features.Tasks.Commands.CreateTask.CreateTaskCommand).Assembly);

// Add validation behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Zentry.Application.Common.ValidationBehavior<,>));

// Add Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
    options.Filters.Add<LoggingActionFilter>();
    options.Filters.Add<ModelValidationFilter>();
    options.Filters.Add<ValidationExceptionFilter>();
});

// Add Action Filters
builder.Services.AddScoped<ApiResponseFilter>();
builder.Services.AddScoped<LoggingActionFilter>();
builder.Services.AddScoped<ModelValidationFilter>();
builder.Services.AddScoped<ValidationExceptionFilter>();

// Add repositories
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<ZentryDbContext>());

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}

// Add custom middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Map Controllers
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ZentryDbContext>();
    await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
}

await app.RunAsync().ConfigureAwait(false);

// Make Program class accessible for testing
public partial class Program { }
