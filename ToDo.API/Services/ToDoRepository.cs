using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NLog.Targets.Wrappers;
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
            return _context.ToDo.OrderBy(t => t.Timestamp).ToList();
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