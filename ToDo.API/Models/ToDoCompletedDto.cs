using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class ToDoCompletedDto
    {
        public bool Completed { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public long TaskId { get; set; }
    }
}