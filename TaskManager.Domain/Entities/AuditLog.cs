

namespace TaskManager.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;  //  "Task", "Project"
        public string Action { get; set; } = string.Empty;      //  "Create", "Update"
        public string PerformedBy { get; set; } = string.Empty; // email ot userId
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }                // free text
    }
}
