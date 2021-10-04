using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ToDo.API.Context;

namespace ToDo.API.Services
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoContext _context;
        private readonly ILogger<ToDoRepository> _logger;

        public ToDoRepository(ToDoContext context, ILogger<ToDoRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IEnumerable<Entities.ToDo> GetToDo()
        {
            return _context.ToDo.OrderBy(t => t.ActiveDate).ToList();
        }

        public IEnumerable<Entities.ToDo> GetToDoByActiveDate(string timestamp)
        {
            return _context.ToDo.Where(t => t.ActiveDate == timestamp).ToList();
        }

        public IEnumerable<Entities.ToDo> GetToDoByCompleted(bool completed)
        {
            return _context.ToDo.Where(t => t.Completed == completed).ToList();
        }

        public IEnumerable<Entities.ToDo> GetToDoByCompletionDateRange(string start, string end)
        {
            return _context.ToDo.Where(t => t.CompletionDate != null && 
                           t.CompletionDate.CompareTo(start)  >= 0 && 
                           t.CompletionDate.CompareTo(end) <= 0).ToList();
        }
        
        public Entities.ToDo GetToDoItem(int id)
        {
            var result = _context.ToDo.FirstOrDefault(t => t.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Checklist item with {id} not found");
            }

            return result;
        }

        public Entities.ToDo AddToDoItem(Entities.ToDo toDo)
        {
            var created = _context.ToDo.Add(toDo).Entity;
            _context.SaveChanges();
            return created;
        }

        public void DeleteToDoItem(Entities.ToDo toDoItem)
        {
            _context.ToDo.Remove(toDoItem);
        }
        
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}