using System.Net;
using System.Net.Http.Json;
using App.DTO.v1_0;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace App.Test
{
    public class ToDoListIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ToDoListIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllToDoLists_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/v1.0/todolists");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task GetSingleToDoList_ReturnsNotFound_IfNonExistent()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/v1.0/todolists/{randomId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostToDoList_CreatesNewListAndReturns201()
        {
            // Arrange
            var newList = new ToDoList
            {
                Name = "Integration Test"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1.0/todolists", newList);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            
            var createdList = await response.Content.ReadFromJsonAsync<ToDoList>();
            Assert.NotNull(createdList);
            Assert.NotEqual(Guid.Empty, createdList!.Id);
            Assert.Equal("Integration Test", createdList.Name);
        }

        [Fact]
        public async Task GetSingleToDoList_ReturnsOk_WhenExists()
        {
            
            var newList = new ToDoList
            {
                Name = "Temp List for GET"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1.0/todolists", newList);
            createResponse.EnsureSuccessStatusCode();

            var createdList = await createResponse.Content.ReadFromJsonAsync<ToDoList>();
            Assert.NotNull(createdList);
            
            var response = await _client.GetAsync($"/api/v1.0/todolists/{createdList!.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var fetched = await response.Content.ReadFromJsonAsync<ToDoList>();
            Assert.NotNull(fetched);
            Assert.Equal(createdList.Id, fetched!.Id);
            Assert.Equal("Temp List for GET", fetched.Name);
        }

        [Fact]
        public async Task PutToDoList_UpdatesAndReturns204()
        {
            var newList = new ToDoList
            {
                Name = "ToUpdate"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1.0/todolists", newList);
            createResponse.EnsureSuccessStatusCode();
            var createdList = await createResponse.Content.ReadFromJsonAsync<ToDoList>();
            Assert.NotNull(createdList);
            
            createdList!.Name = "Updated via PUT";
            
            var putResponse = await _client.PutAsJsonAsync(
                $"/api/v1.0/todolists/{createdList.Id}",
                createdList
            );
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
            
            var getResponse = await _client.GetAsync($"/api/v1.0/todolists/{createdList.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updated = await getResponse.Content.ReadFromJsonAsync<ToDoList>();
            Assert.NotNull(updated);
            Assert.Equal("Updated via PUT", updated!.Name);
        }
    }
}
