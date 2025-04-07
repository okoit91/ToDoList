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
        /// Returns all to do lists.
        /// </summary>
        /// <returns>List of to do lists</returns>
        // GET: api/ToDoLists
        [HttpGet]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int) HttpStatusCode.OK)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<App.DTO.v1_0.ToDoList>>> GetToDoLists()
        {
            var res = await _bll.ToDoLists
                .GetAllSortedAsync();
                
            
            return Ok(res);
        }
        /// <summary>
        /// Returns the to do list with the given id.
        /// </summary>
        /// <param name="id">given ID</param>
        /// <returns>to do list with given ID</returns>
        // GET: api/ToDoLists/5
        [HttpGet("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
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
        /// Updates the to do list with the given id.
        /// </summary>
        /// <param name="id">Given ID</param>
        /// <param name="input">Given object</param>
        /// <returns>No content if updated</returns>
        // PUT: api/ToDoLists/5
        [HttpPut("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.ToDoList>> PutToDoList(Guid id, App.DTO.v1_0.ToDoList input)
        {
            if (id != input.Id)
            {
                return BadRequest();
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
        /// Creates a new to do list.
        /// </summary>
        /// <param name="input">Takes in ToDoList object</param>
        /// <returns>New to do list</returns>
        // POST: api/ToDoLists
        [HttpPost]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int)HttpStatusCode.OK)]
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
        /// Deletes the to do list with the given id.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>No content</returns>
        // DELETE: api/ToDoLists/5
        [HttpDelete("{id}")]
        [ProducesResponseType<IEnumerable<App.DTO.v1_0.ToDoList>>((int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<App.DTO.v1_0.ToDoList>> DeleteToDoList(Guid id)
        {
            var res = await _bll.ToDoLists.FirstOrDefaultAsync(id);
            if (res == null)
            {
                return NotFound();
            }
    
            await _bll.ToDoLists.RemoveAsync(res);
            await _bll.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Returns true if the to do list with the given id exists.
        /// </summary>
        /// <param name="id">Takes in ID</param>
        /// <returns>True if ID exists</returns>
        private async Task<bool> ToDoListExistsAsync(Guid id)
        {
            return await _bll.ToDoLists.ExistsAsync(id);
        }
    }
}
