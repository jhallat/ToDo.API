using System;
using System.Collections;
using System.Collections.Generic;
using ToDo.API.Entities;

namespace ToDo.API.Services
{
    public interface IChecklistAuditRepository
    {
        void AuditDelete(long checklistId, string description);
        void AuditUpdate(long checklistId, string property, string oldValue, string newValue);
        void AuditAdd(long checklistId, string description);
        IEnumerable<CheckListAudit> GetAuditByDateRangeAndProperty(DateTime start, DateTime end, String property);
    }
}