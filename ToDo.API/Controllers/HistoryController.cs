using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDo.API.Models;
using ToDo.API.Services;

namespace ToDo.API.Controllers
{
    [ApiController]
    [Route("api/todo/history")]
    public class HistoryController : ControllerBase
    {
        
        private readonly ILogger<ToDoController> _logger;
        private readonly IToDoRepository _toDoRepository;
        private readonly ITaskHandler _taskHandler;

        public HistoryController(IToDoRepository toDoRepository, 
            ITaskHandler taskHandler, 
            ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _toDoRepository = toDoRepository ?? throw new ArgumentException(nameof(toDoRepository));
            _taskHandler = taskHandler ?? throw new ArgumentNullException(nameof(taskHandler));
        }
        
        [HttpGet("completed/summary")]
        public IActionResult GetCompletionHistory([FromQuery(Name="start")] string start,
                                                  [FromQuery(Name="end")] string end)
        {
            var completedEntities = _toDoRepository.GetToDoByCompletionDateRange(start, end)
                .GroupBy(t => t.CompletionDate);
            List<ToDoCompletedSummary> summaries = new List<ToDoCompletedSummary>();
            int index = 1;
            foreach (var completedEntity in completedEntities)
            {
                var summary = new ToDoCompletedSummary()
                {
                    Index = index,
                    CompletionDate = completedEntity.Key,
                    Count = completedEntity.Count()
                };
                summaries.Add(summary);
                index++;
            }
            return Ok(summaries);
        }
    }
}