using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class ToDoCompletedSummary
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        
        [JsonProperty("completionDate")]
        public string CompletionDate { get; set; }
        
        [JsonProperty("completed")]
        public int Count { get; set; }
    }
}