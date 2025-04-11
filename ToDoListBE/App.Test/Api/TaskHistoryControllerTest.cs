using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Api.ApiControllers;
using App.BLL.DTO;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.DTO.v1_0;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace App.Test.Api
{
    public class TaskHistoryControllerTest
    {
        private readonly IAppBLL _bllMock;
        private readonly IMapper _mapperWeb;
        private readonly TaskHistoriesController _controller;
        private readonly ILogger<TaskHistoriesController> _logger;

        public TaskHistoryControllerTest()
        {
            _logger  = Substitute.For<ILogger<TaskHistoriesController>>();
            _bllMock = Substitute.For<IAppBLL>();
            
            _bllMock.TaskHistories.Returns(Substitute.For<ITaskHistoryService>());
            
            _bllMock.Tasks.Returns(Substitute.For<ITaskService>());
            
            var configWeb = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<App.BLL.DTO.TaskHistory, App.DTO.v1_0.TaskHistory>().ReverseMap();
            });
            _mapperWeb = configWeb.CreateMapper();
            
            _controller = new TaskHistoriesController(_bllMock, _mapperWeb, _logger);
        }

        [Fact]
        public async Task GetTaskHistories_ReturnsOk_WithList()
        {
            // Arrange
            var bllHistories = new List<App.BLL.DTO.TaskHistory>
            {
                new () { Id = Guid.NewGuid(), CurrentTitle = "History1" },
                new () { Id = Guid.NewGuid(), CurrentTitle = "History2" }
            };
            _bllMock.TaskHistories.GetAllSortedAsync().Returns(bllHistories);

            // Act
            var result = await _controller.GetTaskHistories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<App.DTO.v1_0.TaskHistory>>(okResult.Value);
            Assert.Equal(2, items.Count());
            Assert.Equal("History1", items.First().CurrentTitle);
        }

        [Fact]
        public async Task GetTaskHistory_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            _bllMock.TaskHistories.FirstOrDefaultAsync(historyId).Returns((App.BLL.DTO.TaskHistory?)null);

            // Act
            var actionResult = await _controller.GetTaskHistory(historyId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("TaskHistory not found.", notFound.Value);
        }

        [Fact]
        public async Task GetTaskHistory_ReturnsOk_WhenFound()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            var bllHistory = new App.BLL.DTO.TaskHistory
            {
                Id = historyId,
                CurrentTitle = "TestHistory"
            };
            _bllMock.TaskHistories.FirstOrDefaultAsync(historyId).Returns(bllHistory);

            // Act
            var actionResult = await _controller.GetTaskHistory(historyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsType<App.DTO.v1_0.TaskHistory>(okResult.Value);
            Assert.Equal("TestHistory", dto.CurrentTitle);
        }

        [Fact]
        public async Task PutTaskHistory_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var urlId = Guid.NewGuid();
            var input = new App.DTO.v1_0.TaskHistory
            {
                Id = Guid.NewGuid(),
                CurrentTitle = "Mismatch"
            };

            // Act
            var result = await _controller.PutTaskHistory(urlId, input);

            // Assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("URL ID does not match the TaskHistory object's ID.", badReq.Value);
        }

        [Fact]
        public async Task PutTaskHistory_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            var inputDto = new App.DTO.v1_0.TaskHistory { Id = historyId, CurrentTitle = "UpdatedTitle" };

            var bllUpdated = new App.BLL.DTO.TaskHistory { Id = historyId, CurrentTitle = "UpdatedTitle" };
            _bllMock.TaskHistories.UpdateAsync(Arg.Any<App.BLL.DTO.TaskHistory>()).Returns(bllUpdated);

            // Act
            var actionResult = await _controller.PutTaskHistory(historyId, inputDto);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task PutTaskHistory_ReturnsNotFound_WhenBLLReturnsNull()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            var inputDto = new App.DTO.v1_0.TaskHistory { Id = historyId, CurrentTitle = "AnyTitle" };

            _bllMock.TaskHistories.UpdateAsync(Arg.Any<App.BLL.DTO.TaskHistory>())
                .Returns((App.BLL.DTO.TaskHistory?)null);

            // Act
            var actionResult = await _controller.PutTaskHistory(historyId, inputDto);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("TaskHistory not found for the given ID.", notFound.Value);
        }

        [Fact]
        public async Task PutTaskHistory_ThrowsConcurrency_WhenStillExists()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            var inputDto = new App.DTO.v1_0.TaskHistory { Id = historyId, CurrentTitle = "Test" };

            // Throw concurrency
            _bllMock.TaskHistories.UpdateAsync(Arg.Any<App.BLL.DTO.TaskHistory>())
                .Throws(new DbUpdateConcurrencyException());

            // Indicate it still exists
            _bllMock.TaskHistories.ExistsAsync(historyId).Returns(true);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                _controller.PutTaskHistory(historyId, inputDto));
        }

        [Fact]
        public async Task PostTaskHistory_ReturnsCreated()
        {
            // Arrange
            var inputDto = new App.DTO.v1_0.TaskHistory
            {
                Id = Guid.NewGuid(),
                CurrentTitle = "NewHistory"
            };

            var bllAdded = new App.BLL.DTO.TaskHistory
            {
                Id = inputDto.Id,
                CurrentTitle = "NewHistory"
            };

            _bllMock.TaskHistories.Add(Arg.Any<App.BLL.DTO.TaskHistory>())
                .Returns(bllAdded);

            // Act
            var actionResult = await _controller.PostTaskHistory(inputDto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var value = Assert.IsType<App.DTO.v1_0.TaskHistory>(created.Value);
            Assert.Equal("NewHistory", value.CurrentTitle);
        }

        [Fact]
        public async Task DeleteTaskHistory_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            _bllMock.TaskHistories.FirstOrDefaultAsync(historyId).Returns((App.BLL.DTO.TaskHistory?)null);

            // Act
            var actionResult = await _controller.DeleteTaskHistory(historyId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteTaskHistory_ReturnsNoContent_WhenFound()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            var existingBllHistory = new App.BLL.DTO.TaskHistory { Id = historyId, CurrentTitle = "ToDelete" };
            _bllMock.TaskHistories.FirstOrDefaultAsync(historyId).Returns(existingBllHistory);

            // Act
            var actionResult = await _controller.DeleteTaskHistory(historyId);

            // Assert
            Assert.IsType<NoContentResult>(actionResult.Result);
            
            await _bllMock.TaskHistories.Received(1).RemoveAsync(existingBllHistory);
        }

        [Fact]
        public async Task RevertTaskHistory_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            
            // Return null for that ID
            _bllMock.TaskHistories.FirstOrDefaultAsync(historyId).Returns((App.BLL.DTO.TaskHistory?)null);

            // Act
            var actionResult = await _controller.RevertTaskHistory(historyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("TaskHistory not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task RevertTaskHistory_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var historyId = Guid.NewGuid();
            var existingHistory = new App.BLL.DTO.TaskHistory
            {
                Id = historyId,
                CurrentTitle = "OldTitle",
                TaskId = Guid.NewGuid()
            };

            var existingTask = new App.BLL.DTO.Task
            {
                Id = existingHistory.TaskId,
                Title = "OriginalTask",
                IsArchived = true,
                IsCompleted = true
            };

            // Mock the BLL
            _bllMock.TaskHistories.FirstOrDefaultAsync(historyId).Returns(existingHistory);
            _bllMock.Tasks.FirstOrDefaultAsync(existingHistory.TaskId).Returns(existingTask);

            // Act
            var actionResult = await _controller.RevertTaskHistory(historyId);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);

            // Check updates
            Assert.NotNull(existingHistory.RevertedAt);
            Assert.False(existingTask.IsArchived);
            Assert.False(existingTask.IsCompleted);
            Assert.Null(existingTask.CompletedAt);
        }
    }
}
