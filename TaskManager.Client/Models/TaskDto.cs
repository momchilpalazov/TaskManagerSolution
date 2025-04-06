namespace TaskManager.Client.Models
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime? DueDate { get; set; }
    }
}
