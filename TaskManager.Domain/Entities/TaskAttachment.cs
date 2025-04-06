

namespace TaskManager.Domain.Entities
{
    public class TaskAttachment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;
    }

}
