

namespace TaskManager.Application.DTOs
{
    public class FilterTasksDto
    {
        public string? Status { get; set; }         // Пример: "Todo", "Done"
        public string? Priority { get; set; }       // Пример: "High", "Medium"
        public Guid? AssignedToUserId { get; set; } // Филтър по потребител
        public DateTime? DueBefore { get; set; }    // Задачи с краен срок преди дата
    }
}
