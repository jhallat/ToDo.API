using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class ToDoSnoozeDto
    {
        [JsonProperty("days")]
        public int Days { get; set; }
    }
}