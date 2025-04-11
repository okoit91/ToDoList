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
    /// API controller for managing to-do lists.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ToDoListsController : ControllerBase
    {
        private readonly IAppBLL _bll;
        private readonly PublicDTOBllMapper<App.DTO.v1_0.ToDoList, App.BLL.DTO.ToDoList> _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bll"></param>
        /// <param name="autoMapper"></param>
        public ToDoListsController(IAppBLL bll, IMapper autoMapper)
        {
            _bll = bll;
            _mapper = new PublicDTOBllMapper<App.DTO.v1_0.ToDoList, App.BLL.DTO.ToDoList>(autoMapper);
        }
        
        
        /// <summary>
        /// Retrieves all to-do lists with their sub-lists and tasks.
        /// </summary>
        /// <returns>A list of all to-do lists.</returns>
        /// <response code="200">Returns the list of to-do lists.</response>
        // GET: api/ToDoLists
        [HttpGet]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int) HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.ToDoList>>> GetToDoLists()
        {
            var res = await _bll.ToDoLists
                .GetAllSortedAsync();
                
            
            return Ok(res);
        }
        
        
        /// <summary>
        /// Retrieves a specific to-do list by ID.
        /// </summary>
        /// <param name="id">The ID of the to-do list.</param>
        /// <returns>The requested to-do list.</returns>
        /// <response code="200">Returns the requested to-do list.</response>
        /// <response code="404">To-do list with specified ID not found.</response>
        // GET: api/ToDoLists/5
        [HttpGet("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.ToDoList>> GetToDoList(Guid id)
        {
            var res = await _bll.ToDoLists.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map(res));
        }
        
        
        /// <summary>
        /// Updates an existing to-do list.
        /// </summary>
        /// <param name="id">The ID of the to-do list to update.</param>
        /// <param name="input">Updated to-do list data.</param>
        /// <returns>No content on successful update.</returns>
        /// <response code="204">Successfully updated the to-do list.</response>
        /// <response code="400">The provided ID does not match the to-do list ID.</response>
        /// <response code="404">To-do list with specified ID not found.</response>
        // PUT: api/ToDoLists/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.ToDoList>> PutToDoList(Guid id, App.DTO.v1_0.ToDoList input)
        {
            if (id != input.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var res = _mapper.Map(input);
                var updatedInput = await _bll.ToDoLists.UpdateAsync(res);
                if (updatedInput == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ToDoListExistsAsync(id))
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
        /// Creates a new to-do list.
        /// </summary>
        /// <param name="input">The to-do list to create.</param>
        /// <returns>The created to-do list.</returns>
        /// <response code="201">Successfully created the to-do list.</response>
        // POST: api/ToDoLists
        [HttpPost]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int)HttpStatusCode.Created)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.ToDoList>> PostToDoList(App.DTO.v1_0.ToDoList input)
        {
            var res = _mapper.Map(input);
            var createdInput = _bll.ToDoLists.Add(res);
            await _bll.SaveChangesAsync();

            return CreatedAtAction("GetToDoList", new { id = createdInput.Id }, _mapper.Map(createdInput));
        }
        
        
        /// <summary>
        /// Deletes a to-do list and all its related tasks and task histories.
        /// </summary>
        /// <param name="id">The ID of the to-do list to delete.</param>
        /// <returns>No content if deletion was successful.</returns>
        /// <response code="204">Successfully deleted the to-do list and its contents.</response>
        /// <response code="404">To-do list with specified ID not found.</response>
        // DELETE: api/ToDoLists/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteToDoList(Guid id)
        {
            var list = await _bll.ToDoLists.FirstOrDefaultAsync(id);
            if (list == null) return NotFound();
            
            await _bll.ToDoLists.DeleteListAndRelatedDataAsync(id);

            await _bll.SaveChangesAsync();

            return NoContent();
        }
        
        
        /// <summary>
        /// Checks if a to-do list with the specified ID exists.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>True if ID exists</returns>
        private async Task<bool> ToDoListExistsAsync(Guid id)
        {
            return await _bll.ToDoLists.ExistsAsync(id);
        }
    }
}
