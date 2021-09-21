using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.API.Context;
using ToDo.API.Entities;

namespace ToDo.API.Services
{
    public class ChecklistAuditRepository : IChecklistAuditRepository
    {
        private readonly ToDoContext _context;

        public ChecklistAuditRepository(ToDoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }        
        
        public CheckListAudit AddCheckListAudit(CheckListAudit checkListAudit)
        {
            var created = _context.CheckListAudits.Add(checkListAudit).Entity;
            _context.SaveChanges();
            return created;
        }

        public IEnumerable<CheckListAudit> GetAuditByDateRangeAndProperty(DateTime start, DateTime end, string property)
        {
            return _context.CheckListAudits.Where(item => item.AuditDate.CompareTo(start) > 0
                   && item.AuditDate.CompareTo(end) < 0
                   && item.Property.Equals(property)).ToList();
        }
    }
}