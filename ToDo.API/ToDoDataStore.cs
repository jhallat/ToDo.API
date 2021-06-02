using System.Collections.Generic;
using ToDo.API.Models;

namespace ToDo.API
{
    public class ToDoDataStore
    {
        public static ToDoDataStore Current { get; } = new ToDoDataStore();
        public List<ToDoDto> ToDoList { get; set; }

        private ToDoDataStore()
        {
            ToDoList = new List<ToDoDto>()
            {
                new ToDoDto()
                {
                    Id = 1,
                    Timestamp = "20210519",
                    Description = "Plan trip to Morocco",
                    Completed = false
                },
                new ToDoDto()
                {
                    Id = 2,
                    Timestamp = "20210519",
                    Description = "Update resume",
                    Completed = false
                }
            };
        }
        
        
    }
}