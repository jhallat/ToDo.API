using System;
using System.Collections.Generic;
using System.Globalization;

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
        private readonly ITaskHandler _taskHandler;

        public ToDoController(IToDoRepository toDoRepository, 
                              IMapper mapper, 
                              ITaskHandler taskHandler, 
                              ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _toDoRepository = toDoRepository ?? throw new ArgumentException(nameof(toDoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _taskHandler = taskHandler ?? throw new ArgumentNullException(nameof(taskHandler));
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

 
        
        [HttpGet("today")]
        public IActionResult GetToDoItemsForToday()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd");
            try
            {
                var toDoEntities = _toDoRepository.GetToDoByActiveDate(timestamp);
                return Ok(toDoEntities);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured with getting to do item for timestamp {timestamp}.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }
        }

        [HttpGet("incomplete")]
        public IActionResult GetIncompleteToDoItems()
        {
            try
            {
                var toDoEntities = _toDoRepository.GetToDoByCompleted(false);
                return Ok(toDoEntities);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured with getting incomplete to do items.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }
        }
        
        [HttpPost]
        public IActionResult CreateToDoItem(ToDoCreationDto createToDoItem)
        {

            var todoItem = _mapper.Map<Entities.ToDo>(createToDoItem);
            todoItem.ActiveDate = DateTime.Now.ToString("yyyyMMdd");
            todoItem.CreatedDate = todoItem.ActiveDate;
            todoItem.Completed = false;
            var createdToDoItem = _mapper.Map<Models.ToDoDto>(_toDoRepository.AddToDoItem(todoItem));
            if (createToDoItem.TaskId > 0)
            {
                _taskHandler.InProgressTask(new TaskInProgressDto
                {
                    TaskId = createToDoItem.TaskId,
                    InProgress = true
                });
            }
            return CreatedAtRoute("GetToDoItem", new {id = createdToDoItem.Id}, createdToDoItem);

        }

        [HttpPost("quantity-adjustment")]
        public IActionResult AdjustToDoQuantity(ToDoUpdateQuantityDto updateQuantity)
        {
            try
            {
                var todo = _toDoRepository.GetToDoItem(updateQuantity.Id);
                if (todo == null)
                {
                    _logger.LogInformation($"To do item {updateQuantity.Id} not found.");
                    return NotFound();
                }

                int newQuantity = todo.Quantity - updateQuantity.Adjustment;
                if (newQuantity < 0)
                {
                    newQuantity = 0;
                    todo.Completed = true;
                }
                todo.Quantity = newQuantity;
                _toDoRepository.Save();
                if (todo.TaskId > 0 && todo.Completed)
                {
                    _taskHandler.CompleteTask(new TaskCompletedDto
                    {
                        TaskId = todo.TaskId,
                        Completed = todo.Completed
                    });
                }
                return Ok(todo);
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured completing item {updateQuantity.Id}.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }
        }
        
        [HttpPut("{id}/completed")]
        public IActionResult CompleteToDoItem(int id, [FromBody] ToDoCompletedDto toDoCompleted)
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
                todo.CompletionDate = DateTime.Now.ToString("yyyyMMdd");
                _toDoRepository.Save();
                if (toDoCompleted.TaskId > 0)
                {
                    _taskHandler.CompleteTask(new TaskCompletedDto
                    {
                        TaskId = toDoCompleted.TaskId,
                        Completed = toDoCompleted.Completed
                    });
                }
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured completing item {id}.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }

        }
        
        [HttpPut("{id}/active-date")]
        public IActionResult UpdateToDoTimestamp(int id, [FromBody] ToDoUpdateTimestampDto toDoTimestamp)
        {
            try
            {
                var todo = _toDoRepository.GetToDoItem(id);
                if (todo == null)
                {
                    _logger.LogInformation($"To do item {id} not found.");
                    return NotFound();
                }
                todo.ActiveDate = toDoTimestamp.ActiveDate;
                _toDoRepository.Save();
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured completing item {id}.", exception);
                return StatusCode(500, "A problem occured while handling your request.");
            }

        }

        [HttpPost("{id}/snooze")]
        public IActionResult SnoozeToDo(int id, [FromBody] ToDoSnoozeDto toDoSnooze)
        {
            try
            {
                var todo = _toDoRepository.GetToDoItem(id);
                if (todo == null)
                {
                    _logger.LogInformation($"To do item {id} not found.");
                    return NotFound();
                }

                var timestamp = DateTime.ParseExact(todo.ActiveDate,
                    "yyyyMMdd",
                    new CultureInfo("en-US"));

                timestamp = timestamp.AddDays(toDoSnooze.Days);
                todo.ActiveDate = timestamp.ToString("yyyyMMdd");
                _toDoRepository.Save();
                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogCritical($"Exception occured snoozing item {id}.", exception);
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
            {
                _taskHandler.InProgressTask(new TaskInProgressDto
                {
                    TaskId = todo.TaskId,
                    InProgress = false
                });
            }            
            return NoContent();
        }
    }
    
    
}