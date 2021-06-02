using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ToDo.API.Context
{
    public class ToDoContext : DbContext
    {
        public DbSet<Entities.ToDo> ToDo { get; set; }

        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {
            
        }

    }
}