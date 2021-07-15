namespace ToDo.API.Models
{
    public class ToDoDto
    {
        public int Id { get; set; }
        public string ActiveDate { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
        public string CreatedDate { get; set; }
        public string CompletionDate { get; set; }
    }
}