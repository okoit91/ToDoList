using System.Net;
using App.Contracts.BLL;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;

namespace App.Api.ApiControllers
{
    /// <summary>
    /// Controller for managing task entities.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Task, App.BLL.DTO.Task> _mapper;
        private readonly ILogger<TasksController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bll"></param>
        /// <param name="autoMapper"></param>
        public TasksController(IAppBLL bll, IMapper autoMapper, ILogger<TasksController> logger)
        {
            _bll = bll;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Task, App.BLL.DTO.Task>(autoMapper);
            _logger = logger;
        }
        /// <summary>
        /// Retrieves all tasks.
        /// </summary>
        /// <returns>A collection of all tasks.</returns>
        /// <response code="200">Returns all tasks.</response>
        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.Task>), (int) HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.Task>>> GetTasks()
        {
            var bllTasks = await _bll.Tasks.GetAllSortedAsync();
            _logger.LogInformation("Fetched {Count} tasks.", bllTasks.Count());
            
            var mapped = bllTasks.Select(t => _mapper.Map(t)).ToList();
            
            return Ok(mapped);
        }
        
        
        /// <summary>
        /// Retrieves a specific task by ID.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <returns>The requested task.</returns>
        /// <response code="200">Returns the task.</response>
        /// <response code="404">If the task is not found.</response>
        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Task>> GetTask(Guid id)
        {
            var res = await _bll.Tasks.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            _logger.LogInformation("Task {TaskId} fetched successfully.", id);
            return Ok(_mapper.Map(res));
        }
        
        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="id">The task ID to update.</param>
        /// <param name="input">The updated task object.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Update successful.</response>
        /// <response code="400">ID mismatch.</response>
        /// <response code="404">Task not found.</response>
        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutTask(Guid id, App.DTO.v1_0.Task input)
        {
            if (id != input.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            try
            {
                var res = _mapper.Map(input);
                var updatedInput = await _bll.Tasks.UpdateAsync(res);
                if (updatedInput == null)
                {
                    return NotFound();
                }
                _logger.LogInformation("Task {TaskId} updated successfully.", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="input">The task to create.</param>
        /// <returns>The created task.</returns>
        /// <response code="201">Task created.</response>
        // POST: api/Tasks
        [HttpPost]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int)HttpStatusCode.Created)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Task>> PostTask(App.DTO.v1_0.Task input)
        {
            var res = _mapper.Map(input);
            res!.CreatedAt = DateTime.UtcNow;

            var createdInput = _bll.Tasks.Add(res);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = createdInput.Id }, _mapper.Map(createdInput));
        }
        
        /// <summary>
        /// Deletes the specified task and its history.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Task deleted.</response>
        /// <response code="404">Task not found.</response>
        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Task>> DeleteTask(Guid id)
        {
            var task = await _bll.Tasks.FirstOrDefaultAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            
            var historyEntries = await _bll.TaskHistories.GetAllByTaskIdAsync(id);
            foreach (var entry in historyEntries)
            {
                await _bll.TaskHistories.RemoveAsync(entry);
            }

            await _bll.Tasks.RemoveAsync(task);
            await _bll.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} and related history deleted successfully.", id);
            return NoContent();
        }
        /// <summary>
        /// Returns true if the task with the given id exists.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>True if ID exists</returns>
        private async Task<bool> TaskExistsAsync(Guid id)
        {
            return await _bll.Tasks.ExistsAsync(id);
        }
    }
}
