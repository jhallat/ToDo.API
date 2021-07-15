using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class ToDoCompletedSummary
    {
        [JsonProperty("completionDate")]
        public string CompletionDate { get; set; }
        
        [JsonProperty("completed")]
        public int Count { get; set; }
    }
}