using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.API.Context;

namespace ToDo.API.Services
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoContext _context;

        public ToDoRepository(ToDoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            return _context.ToDo.FirstOrDefault(t => t.Id == id);
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