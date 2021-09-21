using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ToDo.API.Models
{
    public class ToDoCreationDto
    {
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description is limited to 255 maximum characters")]
        public string Description { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public long TaskId { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public int Quantity { get; set; }
        public long GoalId { get; set; }
        public string GoalDescription { get; set; }
    }
}