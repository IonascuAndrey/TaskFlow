using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Models {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<UserToTask> UserToTasks { get; set; }
        public DbSet<UserToProject> UserToProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Configure composite keys
            modelBuilder.Entity<UserToTask>()
                .HasKey(ut => new { ut.UserId, ut.TaskId });

            modelBuilder.Entity<UserToProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });

            base.OnModelCreating(modelBuilder);
        }
    }
}