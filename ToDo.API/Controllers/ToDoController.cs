using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDo.API.Entities;
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
        private readonly IChecklistAuditRepository _auditRepository;
        private readonly IMapper _mapper;
        private readonly ITaskHandler _taskHandler;

        public ToDoController(IToDoRepository toDoRepository,
            IChecklistAuditRepository auditRepository,
            IMapper mapper,
            ITaskHandler taskHandler,
            ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _toDoRepository = toDoRepository ?? throw new ArgumentException(nameof(toDoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _taskHandler = taskHandler ?? throw new ArgumentNullException(nameof(taskHandler));
            _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        }

        [HttpGet]
        public IActionResult GetToDo()
        {
            _logger.LogInformation("GET: api/todo");
            var toDoEntities = _toDoRepository.GetToDo();
            return Ok(_mapper.Map<IEnumerable<ToDoDto>>(toDoEntities));
        }

        [HttpGet("{id}", Name = "GetToDoItem")]
        public IActionResult GetToDoItem(int id)
        {
            _logger.LogInformation($"GET: api/todo/{id}");
            var todo = _toDoRepository.GetToDoItem(id);
            return Ok(todo);
        }

        [HttpGet("active/{date}")]
        public IActionResult GetToDoItemsForActiveDate(string date)
        {
            _logger.LogInformation($"GET: api/todo/active/{date}");
            var toDoEntities = _toDoRepository.GetToDoByActiveDate(date);
            return Ok(toDoEntities);
        }

        [HttpGet("today")]
        public IActionResult GetToDoItemsForToday()
        {
            _logger.LogInformation($"GET: api/todo/today");
            var easternZone = TimeZoneInfo.FindSystemTimeZoneById("EST");
            var timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone).ToString("yyyyMMdd");
            var toDoEntities = _toDoRepository.GetToDoByActiveDate(timestamp);
            return Ok(toDoEntities);
        }

        [HttpGet("incomplete")]
        public IActionResult GetIncompleteToDoItems()
        {
            _logger.LogInformation($"GET: api/todo/incomplete");
            var toDoEntities = _toDoRepository.GetToDoByCompleted(false);
            return Ok(toDoEntities);
        }

        [HttpPost]
        public IActionResult CreateToDoItem(ToDoCreationDto createToDoItem)
        {
            _logger.LogInformation($"POST: api/todo/ body={createToDoItem}");
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
            _auditRepository.AuditAdd(createdToDoItem.Id, createdToDoItem.Description);
            return CreatedAtRoute("GetToDoItem", new {id = createdToDoItem.Id}, createdToDoItem);
        }

        [HttpPost("quantity-adjustment")]
        public IActionResult AdjustToDoQuantity(ToDoUpdateQuantityDto updateQuantity)
        {
            _logger.LogInformation($"POST: api/todo/quantity-adjustment body={updateQuantity}");
            var todo = _toDoRepository.GetToDoItem(updateQuantity.Id);
            if (todo == null)
            {
                _logger.LogInformation($"To do item {updateQuantity.Id} not found.");
                return NotFound();
            }

            int newQuantity = todo.Quantity - updateQuantity.Adjustment;
            if (newQuantity <= 0)
            {
                newQuantity = 0;
                todo.Completed = true;
                _auditRepository.AuditUpdate(todo.Id, "Completed", "false", "true");
            }

            var oldQuantity = todo.Quantity;
            todo.Quantity = newQuantity;
            _toDoRepository.Save();
            _auditRepository.AuditUpdate(todo.Id, "Quantity", $"{oldQuantity}", $"{newQuantity}");
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

        [HttpPut("{id}/completed")]
        public IActionResult CompleteToDoItem(int id, [FromBody] ToDoCompletedDto toDoCompleted)
        {
            _logger.LogInformation($"PUT: api/todo/{id}/completed body={toDoCompleted}");
            var todo = _toDoRepository.GetToDoItem(id);

            var oldValue = todo.Completed;
            todo.Completed = toDoCompleted.Completed;
            todo.CompletionDate = DateTime.Now.ToString("yyyyMMdd");
            _toDoRepository.Save();
            _auditRepository.AuditUpdate(todo.Id, "Completed", $"{oldValue}", $"{toDoCompleted.Completed}");
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

        [HttpPut("{id}/active-date")]
        public IActionResult UpdateToDoTimestamp(int id, [FromBody] ToDoUpdateTimestampDto toDoTimestamp)
        {
            _logger.LogInformation($"GET: api/todo/{id}/active-date body={toDoTimestamp}");
            var todo = _toDoRepository.GetToDoItem(id);

            var oldValue = todo.ActiveDate;
            todo.ActiveDate = toDoTimestamp.ActiveDate;
            _toDoRepository.Save();
            _auditRepository.AuditUpdate(todo.Id, "ActiveDate", oldValue, todo.ActiveDate);
            return NoContent();
        }

        [HttpPost("{id}/snooze")]
        public IActionResult SnoozeToDo(int id, [FromBody] ToDoSnoozeDto toDoSnooze)
        {
            _logger.LogInformation($"POST: api/todo/{id}/snooze body={toDoSnooze}");
            var todo = _toDoRepository.GetToDoItem(id);

            var timestamp = DateTime.ParseExact(todo.ActiveDate,
                "yyyyMMdd",
                new CultureInfo("en-US"));

            timestamp = timestamp.AddDays(toDoSnooze.Days);
            var oldValue = todo.ActiveDate;
            todo.ActiveDate = timestamp.ToString("yyyyMMdd");
            _toDoRepository.Save();
            _auditRepository.AuditUpdate(todo.Id, "ActiveDate", oldValue, todo.ActiveDate);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteToDoItem(int id)
        {
            _logger.LogInformation($"DELETE: api/todo/{id}");
            var todo = _toDoRepository.GetToDoItem(id);

            _toDoRepository.DeleteToDoItem(todo);
            _toDoRepository.Save();
            _auditRepository.AuditDelete(todo.Id, todo.Description);
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