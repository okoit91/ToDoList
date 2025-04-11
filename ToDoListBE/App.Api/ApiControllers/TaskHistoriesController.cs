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
    /// Controller responsible for managing TaskHistory records.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TaskHistoriesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.TaskHistory, App.BLL.DTO.TaskHistory> _mapper;
        private readonly ILogger<TaskHistoriesController> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskHistoriesController"/> class.
        /// </summary>
        /// <param name="bll">The BLL interface for accessing business logic.</param>
        /// <param name="autoMapper">The AutoMapper instance for DTO/entity mapping.</param>
        public TaskHistoriesController(IAppBLL bll, IMapper autoMapper, ILogger<TaskHistoriesController> logger)
        {
            _bll = bll;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.TaskHistory, App.BLL.DTO.TaskHistory>(autoMapper);
            _logger = logger;
        }
        
        
        /// <summary>
        /// Retrieves all task history records, sorted by a default logic in the BLL.
        /// </summary>
        /// <returns>A collection of all <see cref="App.DTO.v1_0.TaskHistory"/> objects.</returns>
        /// <response code="200">Returns all task history entries successfully.</response>
        // GET: api/TaskHistories
        [HttpGet]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.TaskHistory>>((int) HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.TaskHistory>>> GetTaskHistories()
        {
            var res = await _bll.TaskHistories
                    
                    .GetAllSortedAsync();
            _logger.LogInformation("Found {Count} TaskHistories.", res.Count());
            
            var mapped = res.Select(t => _mapper.Map(t)).ToList();
            
            return Ok(mapped);
        }
        
        
        /// <summary>
        /// Retrieves a specific TaskHistory by its unique ID.
        /// </summary>
        /// <param name="id">Unique identifier of the TaskHistory.</param>
        /// <returns>The requested <see cref="App.DTO.v1_0.TaskHistory"/> if found.</returns>
        /// <response code="200">Returns the requested TaskHistory object.</response>
        /// <response code="404">TaskHistory with the specified ID was not found.</response>
        // GET: api/TaskHistories/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(App.DTO.v1_0.TaskHistory), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.TaskHistory>> GetTaskHistory(Guid id)
        {
            var res = await _bll.TaskHistories.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound("TaskHistory not found.");
            }
            _logger.LogInformation("TaskHistory {Id} fetched successfully.", id);
            return Ok(_mapper.Map(res));
        }
        
        /// <summary>
        /// Updates an existing TaskHistory with the provided data.
        /// </summary>
        /// <param name="id">The unique ID of the TaskHistory to update.</param>
        /// <param name="input">The updated TaskHistory data.</param>
        /// <returns>NoContent if the update was successful.</returns>
        /// <response code="204">The TaskHistory was successfully updated.</response>
        /// <response code="400">The provided ID does not match the object ID.</response>
        /// <response code="404">No TaskHistory found with the specified ID.</response>
        // PUT: api/TaskHistories/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutTaskHistory(Guid id, App.DTO.v1_0.TaskHistory input)
        {
            if (id != input.Id)
            {
                return BadRequest("URL ID does not match the TaskHistory object's ID.");
            }

            try
            {
                var res = _mapper.Map(input);
                var updatedInput = await _bll.TaskHistories.UpdateAsync(res);
                if (updatedInput == null)
                {
                    return NotFound("TaskHistory not found for the given ID.");
                }
                
                _logger.LogInformation("TaskHistory {Id} updated successfully.", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskHistoryExistsAsync(id))
                {
                    return NotFound("TaskHistory no longer exists.");
                }
                else
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Creates a new TaskHistory record in the database.
        /// </summary>
        /// <param name="input">The TaskHistory data to create.</param>
        /// <returns>The newly created TaskHistory object.</returns>
        /// <response code="201">The TaskHistory was successfully created.</response>
        /// <response code="400">The provided data is invalid.</response>
        // POST: api/TaskHistories
        [HttpPost]
        [ProducesResponseType(typeof(App.DTO.v1_0.TaskHistory), (int)HttpStatusCode.Created)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.TaskHistory>> PostTaskHistory(App.DTO.v1_0.TaskHistory input)
        {
            var res = _mapper.Map(input);
            var createdInput = _bll.TaskHistories.Add(res);
            await _bll.SaveChangesAsync();

            _logger.LogInformation("TaskHistory {Id} created successfully.", createdInput.Id);
            return CreatedAtAction("GetTaskHistory", new { id = createdInput.Id }, _mapper.Map(createdInput));
        }
        
        
        /// <summary>
        /// Deletes the TaskHistory record with the specified ID.
        /// </summary>
        /// <param name="id">The unique ID of the TaskHistory to delete.</param>
        /// <returns>NoContent if deletion is successful.</returns>
        /// <response code="204">Successfully deleted the TaskHistory.</response>
        /// <response code="404">No TaskHistory found with the specified ID.</response>
        // DELETE: api/TaskHistories/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.TaskHistory>> DeleteTaskHistory(Guid id)
        {
            var res = await _bll.TaskHistories.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
    
            await _bll.TaskHistories.RemoveAsync(res);
            await _bll.SaveChangesAsync();

            _logger.LogInformation("TaskHistory {Id} deleted successfully.", id);
            return NoContent();
        }
        
        /// <summary>
        /// Marks the specified TaskHistory entry as reverted and reactivates the associated task.
        /// </summary>
        /// <param name="id">The unique ID of the TaskHistory to revert.</param>
        /// <returns>NoContent if the revert succeeded.</returns>
        /// <response code="204">TaskHistory reverted, and Task was reactivated.</response>
        /// <response code="404">Either TaskHistory or Task was not found.</response>
        [HttpPost("revert/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RevertTaskHistory(Guid id)
        {
            var history = await _bll.TaskHistories.FirstOrDefaultAsync(id);
            if (history == null) return NotFound("TaskHistory not found.");

            
            history.RevertedAt = DateTime.UtcNow;
            _bll.TaskHistories.Update(history);

            
            var task = await _bll.Tasks.FirstOrDefaultAsync(history.TaskId);
            if (task == null) return NotFound();

            task.IsArchived = false;
            task.IsCompleted = false;
            task.CompletedAt = null;
            task.UpdatedAt = DateTime.UtcNow;

            _bll.Tasks.Update(task);
            await _bll.SaveChangesAsync();

            _logger.LogInformation("Task {TaskId} was successfully reactivated from history {HistoryId}.",
                task.Id, id);
            return NoContent();
        }
        
        
        /// <summary>
        /// Checks if a TaskHistory exists for the given ID.
        /// </summary>
        /// <param name="id">The unique identifier to check.</param>
        /// <returns>True if the TaskHistory exists; otherwise false.</returns>
        private async Task<bool> TaskHistoryExistsAsync(Guid id)
        {
            return await _bll.TaskHistories.ExistsAsync(id);
        }
    }
}
