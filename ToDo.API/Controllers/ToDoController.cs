using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDo.API.Models;
using ToDo.API.Services;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("api/todo")]
    public class ToDoController : ControllerBase
    {

        private readonly ILogger<ToDoController> _logger;
        private readonly IToDoRepository _toDoRepository;
        private readonly IMapper _mapper;

        public ToDoController(IToDoRepository toDoRepository, IMapper mapper, ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _toDoRepository = toDoRepository ?? throw new ArgumentException(nameof(toDoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public IActionResult GetToDo()
        {
            var toDoEntities = _toDoRepository.GetToDo();
            return Ok(_mapper.Map<IEnumerable<ToDoDto>>(toDoEntities));
        }

        [HttpGet("{id}", Name="GetToDoItem")]
        public IActionResult GetToDoItem(int id)
        {
            try
            {
                var todo = _toDoRepository.GetToDoItem(id);
                if (todo == null)
                {
                    _logger.LogInformation($"To do item {id} not found.");
                    return NotFound();
                }

                return Ok(todo);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured with getting to do item {id}.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }

        }

        [HttpPost]
        public IActionResult CreateToDoItem(ToDoCreationDto createToDoItem)
        {

            var todoItem = _mapper.Map<Entities.ToDo>(createToDoItem);
            todoItem.Timestamp = DateTime.Now.ToString("yyyyMMdd");
            todoItem.Completed = false;
            var createdToDoItem = _mapper.Map<Models.ToDoDto>(_toDoRepository.AddToDoItem(todoItem));
            
            return CreatedAtRoute("GetToDoItem", new {id = createdToDoItem.Id}, createdToDoItem);

        }

        [HttpPut("{id}/completed")]
        public IActionResult UpdateToDoItem(int id, [FromBody] ToDoCompletedDto toDoCompleted)
        {
            try
            {
                var todo = _toDoRepository.GetToDoItem(id);
                if (todo == null)
                {
                    _logger.LogInformation($"To do item {id} not found.");
                    return NotFound();
                }
                todo.Completed = toDoCompleted.Completed;
                _toDoRepository.Save();
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured completing item {id}.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteToDoItem(int id)
        {
            var todo = _toDoRepository.GetToDoItem(id);
            if (todo == null)
            {
                _logger.LogInformation($"To do item {id} not found.");
                return NotFound();
            }

            _toDoRepository.DeleteToDoItem(todo);
            _toDoRepository.Save();
            return NoContent();
        }
    }
    
    
}