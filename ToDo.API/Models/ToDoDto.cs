namespace ToDo.API.Models
{
    public class ToDoDto
    {
        public int Id { get; set; }
        public string Timestamp { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}