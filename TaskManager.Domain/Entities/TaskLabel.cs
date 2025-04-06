

namespace TaskManager.Domain.Entities
{
    public class TaskLabel
    {
        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }  = null!;

        public Guid LabelId { get; set; } 
        public Label Label { get; set; } = null!;
    }

}
