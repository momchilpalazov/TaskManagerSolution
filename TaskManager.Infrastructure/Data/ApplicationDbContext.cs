using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any additional configuration here
            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.CreatedByUser)
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.AssignedToUser)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaskLabel>()
                .HasKey(tl => new { tl.TaskItemId, tl.LabelId });

            modelBuilder.Entity<TaskLabel>()
                .HasOne(tl => tl.TaskItem)
                .WithMany(t => t.TaskLabels)
                .HasForeignKey(tl => tl.TaskItemId);

            modelBuilder.Entity<TaskLabel>()
                .HasOne(tl => tl.Label)
                .WithMany(l => l.TaskLabels)
                .HasForeignKey(tl => tl.LabelId);





        }
    }
    
}
