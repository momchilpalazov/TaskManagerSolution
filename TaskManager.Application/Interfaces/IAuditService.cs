

namespace TaskManager.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string entity, string action, string performedBy, string? description = null);
    }
}
