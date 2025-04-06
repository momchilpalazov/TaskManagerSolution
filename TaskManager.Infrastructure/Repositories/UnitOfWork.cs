using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Project> Projects { get; }
        public IGenericRepository<TaskItem> TaskItems { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
            Projects = new GenericRepository<Project>(_context);
            TaskItems = new GenericRepository<TaskItem>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
