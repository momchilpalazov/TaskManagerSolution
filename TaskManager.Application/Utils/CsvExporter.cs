

using System.Text;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Utils
{
    public static class CsvExporter
    {
        public static byte[] ExportTasksToCsv(IEnumerable<TaskDto> tasks)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id,Title,Status,Priority,DueDate,AssignedTo");

            foreach (var task in tasks)
            {
                builder.AppendLine($"{task.Id},{Escape(task.Title)},{task.Status},{task.Priority},{task.DueDate?.ToShortDateString()},{task.AssignedToEmail}");
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return value.Replace(",", ";").Replace("\n", " ").Replace("\r", "");
        }
    }
}
