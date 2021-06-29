using System;

namespace ToDo.API.Models
{
    public class TaskCompletedDto
    {
        public long TaskId { get; set; }
        public bool Completed { get; set; }
    }
}