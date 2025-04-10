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
    public class TaskHistoriesController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.TaskHistory, App.BLL.DTO.TaskHistory> _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bll"></param>
        /// <param name="autoMapper"></param>
        public TaskHistoriesController(IAppBLL bll, IMapper autoMapper)
        {
            _bll = bll;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.TaskHistory, App.BLL.DTO.TaskHistory>(autoMapper);
        }
        /// <summary>
        /// Returns all historical tasks.
        /// </summary>
        /// <returns>List of taskHistories</returns>
        // GET: api/TaskHistories
        [HttpGet]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.TaskHistory>>((int) HttpStatusCode.OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.TaskHistory>>> GetTaskHistories()
        {
            var res = await _bll.TaskHistories
                    .GetAllSortedAsync();
            
            return Ok(res);
        }
        
        
        /// <summary>
        /// Returns the taskHistory with the given id.
        /// </summary>
        /// <param name="id">given ID</param>
        /// <returns>TaskHistory with given ID</returns>
        // GET: api/TaskHistories/5
        [HttpGet("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.TaskHistory>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.TaskHistory>> GetTaskHistory(Guid id)
        {
            var res = await _bll.TaskHistories.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map(res));
        }
        
        /// <summary>
        /// Updates the taskHistory with the given id.
        /// </summary>
        /// <param name="id">Given ID</param>
        /// <param name="input">Given object</param>
        /// <returns>No content if updated</returns>
        // PUT: api/TaskHistories/5
        [HttpPut("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.TaskHistory>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutTaskHistory(Guid id, App.DTO.v1_0.TaskHistory input)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }

            try
            {
                var res = _mapper.Map(input);
                var updatedInput = await _bll.TaskHistories.UpdateAsync(res);
                if (updatedInput == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskHistoryExistsAsync(id))
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
        /// Creates a new taskHistory.
        /// </summary>
        /// <param name="input">Takes in TaskHistory object</param>
        /// <returns>New task</returns>
        // POST: api/TaskHistories
        [HttpPost]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.TaskHistory>>((int)HttpStatusCode.OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.TaskHistory>> PostTaskHistory(App.DTO.v1_0.TaskHistory input)
        {
            var res = _mapper.Map(input);
            var createdInput = _bll.TaskHistories.Add(res);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetTaskHistory", new { id = createdInput.Id }, _mapper.Map(createdInput));
        }
        /// <summary>
        /// Deletes the task history with the given id.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>No content</returns>
        // DELETE: api/TaskHistories/5
        [HttpDelete("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.TaskHistory>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.TaskHistory>> DeleteTaskHistory(Guid id)
        {
            var res = await _bll.TaskHistories.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
    
            await _bll.TaskHistories.RemoveAsync(res);
            await _bll.SaveChangesAsync();

            return NoContent();
        }
        
        /// <summary>
        ///  Marks the TaskHistory entry as reverted.
        ///  Task associated with it will be reactivated as an undone task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("revert/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RevertTaskHistory(Guid id)
        {
            var history = await _bll.TaskHistories.FirstOrDefaultAsync(id);
            if (history == null) return NotFound();

            
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

            return NoContent();
        }
        
        
        /// <summary>
        /// Returns true if the task history with the given id exists.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>True if ID exists</returns>
        private async Task<bool> TaskHistoryExistsAsync(Guid id)
        {
            return await _bll.TaskHistories.ExistsAsync(id);
        }
    }
}
