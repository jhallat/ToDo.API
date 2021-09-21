using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Models
{
    public class ToDoUpdateQuantityDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
        public int Adjustment { get; set; }
    }
}