using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Models
{
    public class ToDoUpdateTimestampDto
    {
        [Required(ErrorMessage = "Timestamp is required")]
        [MaxLength(8, ErrorMessage = "Timestamp must be 8 characters")]
        [MinLength(8, ErrorMessage = "Timestamp must be 8 characters")]
        public string Timestamp { get; set; }
    }
}