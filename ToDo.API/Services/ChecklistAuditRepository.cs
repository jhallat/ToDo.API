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
        
        private void AddCheckListAudit(CheckListAudit checkListAudit)
        {
            var created = _context.CheckListAudits.Add(checkListAudit).Entity;
            _context.SaveChanges();
        }

        public IEnumerable<CheckListAudit> GetAuditByDateRangeAndProperty(DateTime start, DateTime end, string property)
        {
            return _context.CheckListAudits.Where(item => item.AuditDate.CompareTo(start) > 0
                   && item.AuditDate.CompareTo(end) < 0
                   && item.Property.Equals(property)).ToList();
        }

        public void AuditAdd(long checklistId, string description)
        {
            var checkListAudit = new CheckListAudit
            {
                AuditAction = AuditActions.ADD,
                AuditDate = DateTime.Now,
                ChecklistId = checklistId,
                Id = 0,
                NewValue = $"{checklistId}:{description}",
                OriginalValue = "",
                Property = ""
            };
        }
        
        public void AuditUpdate(long checklistId, string property, string oldValue, string newValue)
        {
            AddCheckListAudit(new CheckListAudit
            {
                AuditAction = AuditActions.UPDATE,
                AuditDate = DateTime.Now,
                ChecklistId = checklistId,
                Id = 0,
                NewValue = newValue,
                OriginalValue = oldValue,
                Property = property
            });
        }
        
        public void AuditDelete(long checklistId, string description)
        {
            AddCheckListAudit(new CheckListAudit
            {
                AuditAction = AuditActions.DELETE,
                AuditDate = DateTime.Now,
                ChecklistId = checklistId,
                Id = 0,
                NewValue = "",
                OriginalValue = $"{checklistId}:{description}",
                Property = ""
            });
        }
    }
}