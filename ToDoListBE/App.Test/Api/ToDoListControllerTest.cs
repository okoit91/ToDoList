using App.Api.ApiControllers;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
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
    public class ToDoListControllerTest
    {
        private readonly IAppBLL _bllMock;
        private readonly IMapper _mapperWeb;
        private readonly ToDoListsController _controller;
        private readonly ILogger<ToDoListsController> _logger;

        public ToDoListControllerTest()
        {
            _logger  = Substitute.For<ILogger<ToDoListsController>>();
            
            _bllMock = Substitute.For<IAppBLL>();
            
            _bllMock.ToDoLists.Returns(Substitute.For<IToDoListService>());
            
            var configWeb = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<App.BLL.DTO.ToDoList, App.DTO.v1_0.ToDoList>().ReverseMap();
            });
            _mapperWeb = configWeb.CreateMapper();
            
            _controller = new ToDoListsController(_bllMock, _mapperWeb, _logger);
        }

        [Fact]
        public async Task GetToDoLists_ReturnsOk_WithList()
        {
            // Arrange
            var sampleBLLLists = new List<App.BLL.DTO.ToDoList>()
            {
                new() { Id = Guid.NewGuid(), Name = "Shopping" },
                new() { Id = Guid.NewGuid(), Name = "Work Tasks" },
            };
            
            _bllMock.ToDoLists.GetAllSortedAsync().Returns(sampleBLLLists);

            // Act
            var result = await _controller.GetToDoLists();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<App.DTO.v1_0.ToDoList>>(okResult.Value);
            var list = returnValue.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("Shopping", list[0].Name);
        }

        [Fact]
        public async Task GetToDoList_ReturnsNotFound_WhenNoList()
        {
            // Arrange
            var listId = Guid.NewGuid();
            _bllMock.ToDoLists.FirstOrDefaultAsync(listId).Returns((App.BLL.DTO.ToDoList?)null);

            // Act
            var actionResult = await _controller.GetToDoList(listId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetToDoList_ReturnsOk_WhenFound()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var bllList = new App.BLL.DTO.ToDoList { Id = listId, Name = "TestList" };
            _bllMock.ToDoLists.FirstOrDefaultAsync(listId).Returns(bllList);

            // Act
            var actionResult = await _controller.GetToDoList(listId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsType<App.DTO.v1_0.ToDoList>(okResult.Value);
            Assert.Equal("TestList", dto.Name);
        }

        [Fact]
        public async Task PutToDoList_ReturnsBadRequest_OnIdMismatch()
        {
            // Arrange
            var urlId = Guid.NewGuid();
            var input = new App.DTO.v1_0.ToDoList
            {
                Id = Guid.NewGuid(),
                Name = "MismatchName"
            };

            // Act
            var result = await _controller.PutToDoList(urlId, input);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ID mismatch", badRequest.Value);
        }

        [Fact]
        public async Task PutToDoList_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var inputDto = new App.DTO.v1_0.ToDoList { Id = listId, Name = "UpdatedName" };
            
            var updatedBllList = new App.BLL.DTO.ToDoList { Id = listId, Name = "UpdatedName" };
            _bllMock.ToDoLists.UpdateAsync(Arg.Any<App.BLL.DTO.ToDoList>()).Returns(updatedBllList);

            // Act
            var actionResult = await _controller.PutToDoList(listId, inputDto);

            // Assert
            Assert.IsType<NoContentResult>(actionResult.Result);
        }

        [Fact]
        public async Task PutToDoList_ReturnsNotFound_WhenUpdateAsyncReturnsNull()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var inputDto = new App.DTO.v1_0.ToDoList { Id = listId, Name = "NewName" };
            _bllMock.ToDoLists.UpdateAsync(Arg.Any<App.BLL.DTO.ToDoList>()).Returns((App.BLL.DTO.ToDoList?)null);

            // Act
            var actionResult = await _controller.PutToDoList(listId, inputDto);

            // Assert
            var notFound = Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PutToDoList_ReturnsNotFound_OnDbUpdateConcurrencyExceptionAndNoListExists()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var inputDto = new App.DTO.v1_0.ToDoList { Id = listId, Name = "NewName" };

            // Throw concurrency exception
            _bllMock.ToDoLists.UpdateAsync(Arg.Any<App.BLL.DTO.ToDoList>())
                .Throws(new DbUpdateConcurrencyException());

            // list doesnt exist anymore
            _bllMock.ToDoLists.ExistsAsync(listId).Returns(false);

            // Act
            var actionResult = await _controller.PutToDoList(listId, inputDto);

            // Assert
            var notFound = Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostToDoList_ReturnsCreatedAtAction()
        {
            // Arrange
            var newList = new App.DTO.v1_0.ToDoList { Id = Guid.NewGuid(), Name = "NewList" };
            var bllList = new App.BLL.DTO.ToDoList { Id = newList.Id, Name = "NewList" };

            _bllMock.ToDoLists.Add(Arg.Any<App.BLL.DTO.ToDoList>()).Returns(bllList);

            // Act
            var actionResult = await _controller.PostToDoList(newList);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<App.DTO.v1_0.ToDoList>(createdResult.Value);
            Assert.Equal(newList.Name, returnValue.Name);
        }

        [Fact]
        public async Task DeleteToDoList_ReturnsNotFound_WhenListNull()
        {
            // Arrange
            var listId = Guid.NewGuid();

            // returns null for id
            _bllMock.ToDoLists.FirstOrDefaultAsync(listId).Returns((App.BLL.DTO.ToDoList?)null);

            // Act
            var result = await _controller.DeleteToDoList(listId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteToDoList_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var listId = Guid.NewGuid();
            
            // returns null for id
            var existingList = new App.BLL.DTO.ToDoList { Id = listId, Name = "ToBeDeleted" };
            _bllMock.ToDoLists.FirstOrDefaultAsync(listId).Returns(existingList);

            // Act
            var actionResult = await _controller.DeleteToDoList(listId);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
            // We could also verify if _bll.ToDoLists.DeleteListAndRelatedDataAsync(...) was called, e.g.:
            await _bllMock.ToDoLists.Received(1).DeleteListAndRelatedDataAsync(listId);
        }
    }
}
