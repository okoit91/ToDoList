using System.Net;
using System.Net.Http.Json;
using App.Domain; // or your DTO namespace
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace App.Test;

// Use the same custom factory that uses the in-memory DB
public class TasksIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TasksIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_WhenNoneExist_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/tasks");
        response.EnsureSuccessStatusCode();

        // Assert
        var tasks = await response.Content.ReadFromJsonAsync<List<App.DTO.v1_0.Task>>();
        Assert.Empty(tasks!);
    }

    [Fact]
    public async Task PostTask_ThenGetById_ReturnsTheSameTask()
    {
        // Arrange
        var newTask = new App.DTO.v1_0.Task
        {
            Title = "New Task",
            Description = "Integration test",
            ToDoListId = Guid.NewGuid() // Or set an existing to-do list if your domain requires
        };

        // Act – create the task
        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/tasks", newTask);
        createResponse.EnsureSuccessStatusCode();
        
        // Assert – it should return 201 + the created Task
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var createdTask = await createResponse.Content.ReadFromJsonAsync<App.DTO.v1_0.Task>();
        Assert.NotNull(createdTask);
        Assert.NotEqual(Guid.Empty, createdTask!.Id);
        Assert.Equal(newTask.Title, createdTask.Title);

        // Act again – GET by ID
        var getResponse = await _client.GetAsync($"/api/v1.0/tasks/{createdTask.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedTask = await getResponse.Content.ReadFromJsonAsync<App.DTO.v1_0.Task>();

        // Assert – confirm it matches
        Assert.NotNull(fetchedTask);
        Assert.Equal(createdTask.Id, fetchedTask!.Id);
        Assert.Equal(createdTask.Title, fetchedTask.Title);
    }

    [Fact]
    public async Task PutTask_UpdatesCorrectly()
    {
        // 1) Create a new task
        var newTask = new App.DTO.v1_0.Task
        {
            Title = "Task to update",
            Description = "Before Update"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/tasks", newTask);
        createResponse.EnsureSuccessStatusCode();

        var createdTask = await createResponse.Content.ReadFromJsonAsync<App.DTO.v1_0.Task>();
        Assert.NotNull(createdTask);

        // 2) Update the task
        createdTask!.Description = "After Update";
        
        var putResponse = await _client.PutAsJsonAsync($"/api/v1.0/tasks/{createdTask.Id}", createdTask);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // 3) GET again to confirm
        var getResponse = await _client.GetAsync($"/api/v1.0/tasks/{createdTask.Id}");
        getResponse.EnsureSuccessStatusCode();
        var updatedTask = await getResponse.Content.ReadFromJsonAsync<App.DTO.v1_0.Task>();

        Assert.NotNull(updatedTask);
        Assert.Equal("After Update", updatedTask!.Description);
    }
    
}
