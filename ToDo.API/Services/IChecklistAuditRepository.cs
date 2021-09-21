using System;
using System.Collections;
using System.Collections.Generic;
using ToDo.API.Entities;

namespace ToDo.API.Services
{
    public interface IChecklistAuditRepository
    {
        CheckListAudit AddCheckListAudit(CheckListAudit checkListAudit);
        IEnumerable<CheckListAudit> GetAuditByDateRangeAndProperty(DateTime start, DateTime end, String property);
    }
}