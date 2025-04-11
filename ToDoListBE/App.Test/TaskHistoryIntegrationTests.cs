using System.Net;
using System.Net.Http.Json;
using App.DTO.v1_0;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace App.Test;

public class TaskHistoryIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaskHistoryIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTaskHistories_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1.0/taskhistories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTaskHistory_ReturnsNotFound_ForInvalidId()
    {
        var response = await _client.GetAsync($"/api/v1.0/taskhistories/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostTaskHistory_CreatesAndReturns201()
    {
        var newHistory = new TaskHistory
        {
            CurrentTitle = "Initial",
            TaskId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/v1.0/taskhistories", newHistory);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.True(false, $"Post failed: {error}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<TaskHistory>();
        Assert.NotNull(created);
        Assert.Equal("Initial", created!.CurrentTitle);
    }

    [Fact]
    public async Task PutTaskHistory_UpdatesAndReturns204()
    {
        // create new
        var taskHistory = new TaskHistory
        {
            CurrentTitle = "ToUpdate",
            TaskId = Guid.NewGuid()
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1.0/taskhistories", taskHistory);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskHistory>();
        Assert.NotNull(created);

        // Update
        created!.CurrentTitle = "Updated";

        var putResponse = await _client.PutAsJsonAsync(
            $"/api/v1.0/taskhistories/{created.Id}", created);

        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // Fetch and confirm
        var getResponse = await _client.GetAsync($"/api/v1.0/taskhistories/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<TaskHistory>();

        Assert.NotNull(fetched);
        Assert.Equal("Updated", fetched!.CurrentTitle);
    }
    

    [Fact]
    public async Task RevertTaskHistory_ReactivatesTask()
    {
        var task = new App.DTO.v1_0.Task
        {
            Title = "Task for history",
            IsCompleted = true,
            IsArchived = true
        };
        var taskResponse = await _client.PostAsJsonAsync("/api/v1.0/tasks", task);
        taskResponse.EnsureSuccessStatusCode();

        var createdTask = await taskResponse.Content.ReadFromJsonAsync<App.DTO.v1_0.Task>();
        Assert.NotNull(createdTask);

        var history = new TaskHistory
        {
            TaskId = createdTask!.Id,
            CurrentTitle = createdTask.Title,
            CompletedAt = DateTime.UtcNow
        };

        var historyResponse = await _client.PostAsJsonAsync("/api/v1.0/taskhistories", history);
        var createdHistory = await historyResponse.Content.ReadFromJsonAsync<TaskHistory>();
        Assert.NotNull(createdHistory);

        
        var revertResponse = await _client.PostAsync($"/api/v1.0/taskhistories/revert/{createdHistory!.Id}", null);
        Assert.Equal(HttpStatusCode.NoContent, revertResponse.StatusCode);

        
        var getTask = await _client.GetAsync($"/api/v1.0/tasks/{createdTask.Id}");
        var updatedTask = await getTask.Content.ReadFromJsonAsync<App.DTO.v1_0.Task>();
        Assert.NotNull(updatedTask);
        Assert.False(updatedTask!.IsArchived);
        Assert.False(updatedTask!.IsCompleted);
        Assert.Null(updatedTask!.CompletedAt);
    }
}
