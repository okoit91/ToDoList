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
    public class TaskControllerTest
    {
        private readonly IAppBLL _bllMock;
        private readonly IMapper _mapperWeb;
        private readonly TasksController _controller;
        private ILogger<TasksController> _logger;

        public TaskControllerTest()
        {
            _logger = Substitute.For<ILogger<TasksController>>();
            
            _bllMock = Substitute.For<IAppBLL>();
            
            _bllMock.Tasks.Returns(Substitute.For<ITaskService>());
            
            _bllMock.TaskHistories.Returns(Substitute.For<ITaskHistoryService>());

            
            var configWeb = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<App.BLL.DTO.Task, App.DTO.v1_0.Task>().ReverseMap();
            });
            _mapperWeb = configWeb.CreateMapper();
            
            _controller = new TasksController(_bllMock, _mapperWeb, _logger);
        }

        [Fact]
        public async Task GetTasks_ReturnsOk_WithList()
        {
            // Arrange
            var sampleBllTasks = new List<App.BLL.DTO.Task>
            {
                new() { Id = Guid.NewGuid(), Title = "Task 1" },
                new() { Id = Guid.NewGuid(), Title = "Task 2" },
            };

            _bllMock.Tasks.GetAllSortedAsync().Returns(sampleBllTasks);

            // Act
            var result = await _controller.GetTasks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            
            var tasks = Assert.IsAssignableFrom<IEnumerable<App.DTO.v1_0.Task>>(okResult.Value);
            Assert.Equal(2, tasks.Count());
            Assert.Equal("Task 1", tasks.First().Title);
        }

        [Fact]
        public async Task GetTask_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            // BLL returns null
            _bllMock.Tasks.FirstOrDefaultAsync(taskId).Returns((App.BLL.DTO.Task?)null);

            // Act
            var actionResult = await _controller.GetTask(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetTask_ReturnsOk_WhenFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var bllTask = new App.BLL.DTO.Task
            {
                Id = taskId,
                Title = "Test Task"
            };
            _bllMock.Tasks.FirstOrDefaultAsync(taskId).Returns(bllTask);

            // Act
            var actionResult = await _controller.GetTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsType<App.DTO.v1_0.Task>(okResult.Value);
            Assert.Equal("Test Task", dto.Title);
        }

        [Fact]
        public async Task PutTask_ReturnsBadRequest_OnIdMismatch()
        {
            // Arrange
            var urlId = Guid.NewGuid();
            var input = new App.DTO.v1_0.Task
            {
                Id = Guid.NewGuid(), // mismatch
                Title = "SomeTitle"
            };

            // Act
            var result = await _controller.PutTask(urlId, input);

            // Assert
            var badReq = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID in URL does not match ID in body.", badReq.Value);
        }

        [Fact]
        public async Task PutTask_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var input = new App.DTO.v1_0.Task
            {
                Id = taskId,
                Title = "UpdatedTitle"
            };

            // Mock update
            var updatedBllTask = new App.BLL.DTO.Task { Id = taskId, Title = "UpdatedTitle" };
            _bllMock.Tasks.UpdateAsync(Arg.Any<App.BLL.DTO.Task>()).Returns(updatedBllTask);

            // Act
            var actionResult = await _controller.PutTask(taskId, input);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task PutTask_ReturnsNotFound_WhenBLLUpdateNull()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var input = new App.DTO.v1_0.Task { Id = taskId, Title = "NotFoundTitle" };

            // BLL returns null => not found
            _bllMock.Tasks.UpdateAsync(Arg.Any<App.BLL.DTO.Task>()).Returns((App.BLL.DTO.Task?)null);

            // Act
            var actionResult = await _controller.PutTask(taskId, input);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task PutTask_Throws_OnConcurrencyExceptionIfStillExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var input = new App.DTO.v1_0.Task
            {
                Id = taskId,
                Title = "WillThrow"
            };
            
            _bllMock.Tasks.UpdateAsync(Arg.Any<App.BLL.DTO.Task>())
                .Throws(new DbUpdateConcurrencyException());
            
            _bllMock.Tasks.ExistsAsync(taskId).Returns(true);

            // Act + Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
                _controller.PutTask(taskId, input)
            );
        }

        [Fact]
        public async Task PostTask_ReturnsCreated()
        {
            // Arrange
            var input = new App.DTO.v1_0.Task
            {
                Id = Guid.NewGuid(),
                Title = "NewTask"
            };

            var bllAddedTask = new App.BLL.DTO.Task
            {
                Id = input.Id,
                Title = "NewTask"
            };
            _bllMock.Tasks.Add(Arg.Any<App.BLL.DTO.Task>()).Returns(bllAddedTask);

            // Act
            var actionResult = await _controller.PostTask(input);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<App.DTO.v1_0.Task>(created.Value);
            Assert.Equal("NewTask", returnValue.Title);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            // BLL returns null for this ID
            _bllMock.Tasks.FirstOrDefaultAsync(taskId).Returns((App.BLL.DTO.Task?)null);

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNoContent_WhenFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingBllTask = new App.BLL.DTO.Task
            {
                Id = taskId,
                Title = "ToDelete"
            };
            _bllMock.Tasks.FirstOrDefaultAsync(taskId).Returns(existingBllTask);

            // For history deletion
            _bllMock.TaskHistories.GetAllByTaskIdAsync(taskId).Returns(new List<App.BLL.DTO.TaskHistory>());

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
            // Validate the BLL calls
            await _bllMock.Tasks.Received(1).RemoveAsync(existingBllTask);
            await _bllMock.TaskHistories.Received(1).GetAllByTaskIdAsync(taskId);
        }
    }
}
