using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid AssignedToUserId { get; set; }
        public User AssignedToUser { get; set; } = null!;

        public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
        public ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();


    }
}
