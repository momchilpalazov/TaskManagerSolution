

namespace TaskManager.Application.DTOs
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AssignedToUserId { get; set; }
        public string Status { get; set; } = "Todo";     // default
        public string Priority { get; set; } = "Medium"; // default
        public DateTime? DueDate { get; set; }
        public List<Guid> LabelIds { get; set; } = new();

    }
}
