
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Project> Projects { get; }
        IGenericRepository<TaskItem> TaskItems { get; }

        Task<int> SaveChangesAsync();
    }
}
