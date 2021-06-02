using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Models
{
    public class ToDoCreationDto
    {
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description is limited to 255 maximum characters")]
        public string Description { get; set; }

    }
}