

namespace TaskManager.Domain.Entities
{
    public class Label
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
    }

}
