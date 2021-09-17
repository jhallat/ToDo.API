using Microsoft.EntityFrameworkCore;
using ToDo.API.Entities;

namespace ToDo.API.Context
{
    public class CheckListAuditContext : DbContext
    {
        public DbSet<CheckListAudit> CheckListAudits { get; set; }

        public CheckListAuditContext(DbContextOptions<CheckListAuditContext> options) : base(options)
        {
            
        }
    }
}