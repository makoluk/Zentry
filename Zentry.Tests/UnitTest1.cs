using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Zentry.Infrastructure.Data;
using Zentry.Application.DTOs;

namespace Zentry.Tests;

public class TaskIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TaskIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ZentryDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<ZentryDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Create_Task_ShouldPersist_And_ReturnDto()
    {
        // Arrange
        var createDto = new TaskCreateDto
        {
            Title = "Test Task",
            Description = "Test Description",
            CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111")
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var taskDto = await response.Content.ReadFromJsonAsync<TaskDto>();
        taskDto.Should().NotBeNull();
        taskDto!.Title.Should().Be(createDto.Title);
        taskDto.Description.Should().Be(createDto.Description);
        taskDto.IsDone.Should().BeFalse();
        taskDto.CategoryId.Should().Be(createDto.CategoryId);
        taskDto.Id.Should().NotBeEmpty();
        taskDto.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task List_Tasks_ShouldFilterByIsDone_And_Search()
    {
        // Arrange
        var tasks = new[]
        {
            new TaskCreateDto { Title = "Completed Task", Description = "Done task" },
            new TaskCreateDto { Title = "Pending Task", Description = "Not done task" },
            new TaskCreateDto { Title = "Another Task", Description = "Another description" }
        };

        // Create tasks
        foreach (var task in tasks)
        {
            await _client.PostAsJsonAsync("/api/v1/tasks", task);
        }

        // Mark first task as done
        var listResponse = await _client.GetAsync("/api/v1/tasks");
        var listResult = await listResponse.Content.ReadFromJsonAsync<JsonElement>();
        var firstTaskId = listResult.GetProperty("items")[0].GetProperty("id").GetGuid();
        await _client.PatchAsync($"/api/v1/tasks/{firstTaskId}/toggle", null);

        // Act - Filter by completed tasks
        var completedResponse = await _client.GetAsync("/api/v1/tasks?isDone=true");
        var completedResult = await completedResponse.Content.ReadFromJsonAsync<JsonElement>();
        var completedTasks = completedResult.GetProperty("items").GetArrayLength();

        // Act - Search by title
        var searchResponse = await _client.GetAsync("/api/v1/tasks?search=Another");
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<JsonElement>();
        var searchTasks = searchResult.GetProperty("items").GetArrayLength();

        // Assert
        completedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        completedTasks.Should().Be(1);

        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        searchTasks.Should().Be(1);
    }

    [Fact]
    public async Task Toggle_Task_ShouldFlip_IsDone_And_UpdateTimestamp()
    {
        // Arrange
        var createDto = new TaskCreateDto
        {
            Title = "Toggle Test Task",
            Description = "Test Description"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", createDto);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();
        var taskId = createdTask!.Id;

        // Act - Toggle to done
        var toggleResponse = await _client.PatchAsync($"/api/v1/tasks/{taskId}/toggle", null);
        var toggledTask = await toggleResponse.Content.ReadFromJsonAsync<TaskDto>();

        // Act - Toggle back to not done
        var toggleBackResponse = await _client.PatchAsync($"/api/v1/tasks/{taskId}/toggle", null);
        var toggledBackTask = await toggleBackResponse.Content.ReadFromJsonAsync<TaskDto>();

        // Assert
        toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        toggledTask!.IsDone.Should().BeTrue();
        toggledTask.UpdatedAtUtc.Should().BeAfter(createdTask.UpdatedAtUtc!.Value);

        toggleBackResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        toggledBackTask!.IsDone.Should().BeFalse();
        toggledBackTask.UpdatedAtUtc.Should().BeAfter(toggledTask.UpdatedAtUtc!.Value);
    }
}
