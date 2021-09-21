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
        private readonly IChecklistAuditRepository _auditRepository;
        private readonly ITaskHandler _taskHandler;

        public HistoryController(IChecklistAuditRepository auditRepository, 
            ITaskHandler taskHandler, 
            ILogger<ToDoController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _auditRepository = auditRepository ?? throw new ArgumentException(nameof(auditRepository));
            _taskHandler = taskHandler ?? throw new ArgumentNullException(nameof(taskHandler));
        }
        
        [HttpGet("completed/summary")]
        public IActionResult GetCompletionHistory([FromQuery(Name="start")] DateTime start,
                                                  [FromQuery(Name="end")] DateTime end)
        {
            var completedEntities = _auditRepository.GetAuditByDateRangeAndProperty(
                    start.Subtract(TimeSpan.FromDays(1)), 
                    end.Add(TimeSpan.FromDays(1)),
                    "Completed")
                .GroupBy(t => t.AuditDate.ToString("yyyyMMdd"))
                .Select(item => new
                {
                    AuditDate = item.Key,
                    Completed = item.Sum(a => a.NewValue.ToLower().Equals("true") ? 1 : -1)
                })
                .OrderBy(result => result.AuditDate)
                .Select(result => result);
                
            var summaries = new List<ToDoCompletedSummary>();
            var index = 1;
            foreach (var completedEntity in completedEntities)
            {
                var summary = new ToDoCompletedSummary()
                {
                    Index = index,
                    CompletionDate = completedEntity.AuditDate,
                    Count = completedEntity.Completed
                };
                summaries.Add(summary);
                index++;
            }
            return Ok(summaries);
        }
    }
}