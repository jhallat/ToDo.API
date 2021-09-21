using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class TaskInProgressDto
    {
        [JsonProperty("taskId")]
        public long TaskId { get; set; }
        
        [JsonProperty("inProgress")]
        public bool InProgress { get; set; }
    }
}