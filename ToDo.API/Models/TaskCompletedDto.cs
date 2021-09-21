using System;
using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class TaskCompletedDto
    {
        [JsonProperty("taskId")]
        public long TaskId { get; set; }
        
        [JsonProperty("completed")]
        public bool Completed { get; set; }
    }
}