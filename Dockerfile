# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["Zentry.Api/Zentry.Api.csproj", "Zentry.Api/"]
COPY ["Zentry.Application/Zentry.Application.csproj", "Zentry.Application/"]
COPY ["Zentry.Domain/Zentry.Domain.csproj", "Zentry.Domain/"]
COPY ["Zentry.Infrastructure/Zentry.Infrastructure.csproj", "Zentry.Infrastructure/"]
COPY ["Zentry.Modules.Tasks/Zentry.Modules.Tasks.csproj", "Zentry.Modules.Tasks/"]
COPY ["Zentry.Modules.Notes/Zentry.Modules.Notes.csproj", "Zentry.Modules.Notes/"]

RUN dotnet restore "Zentry.Api/Zentry.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/Zentry.Api"
RUN dotnet build "Zentry.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Zentry.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Zentry.Api.dll"]
