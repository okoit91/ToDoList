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
    /// 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.Task, App.BLL.DTO.Task> _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bll"></param>
        /// <param name="autoMapper"></param>
        public TasksController(IAppBLL bll, IMapper autoMapper)
        {
            _bll = bll;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Task, App.BLL.DTO.Task>(autoMapper);
        }
        /// <summary>
        /// Returns all tasks.
        /// </summary>
        /// <returns>List of tasks</returns>
        // GET: api/Tasks
        [HttpGet]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int) HttpStatusCode.OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.Task>>> GetTasks()
        {
            var res = await _bll.Tasks
                .GetAllSortedAsync();
                
            
            return Ok(res);
        }
        
        
        /// <summary>
        /// Returns the task with the given id.
        /// </summary>
        /// <param name="id">given ID</param>
        /// <returns>Task with given ID</returns>
        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Task>> GetTask(Guid id)
        {
            var res = await _bll.Tasks.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map(res));
        }
        
        /// <summary>
        /// Updates the task with the given id.
        /// </summary>
        /// <param name="id">Given ID</param>
        /// <param name="input">Given object</param>
        /// <returns>No content if updated</returns>
        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutTask(Guid id, App.DTO.v1_0.Task input)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }

            try
            {
                var res = _mapper.Map(input);
                var updatedInput = await _bll.Tasks.UpdateAsync(res);
                if (updatedInput == null)
                {
                    return NotFound();
                }
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
        /// <param name="input">Takes in Task object</param>
        /// <returns>New task</returns>
        // POST: api/Tasks
        [HttpPost]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int)HttpStatusCode.OK)]
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
        /// Deletes the task with the given id.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>No content</returns>
        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.Task>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.Task>> DeleteTask(Guid id)
        {
            var task = await _bll.Tasks.FirstOrDefaultAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Optionally remove related TaskHistory entries first
            var historyEntries = await _bll.TaskHistories.GetAllByTaskIdAsync(id);
            foreach (var entry in historyEntries)
            {
                await _bll.TaskHistories.RemoveAsync(entry);
            }

            await _bll.Tasks.RemoveAsync(task);
            await _bll.SaveChangesAsync();

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
