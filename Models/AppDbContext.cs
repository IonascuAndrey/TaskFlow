using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Models {
    public class AppDbContext : IdentityDbContext<ApplicationUser> {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<UserToTask> UserToTasks { get; set; }
        public DbSet<UserToProject> UserToProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserToTask>()
        .HasKey(ut => new { ut.UserId, ut.TaskId });

            modelBuilder.Entity<UserToProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });

            modelBuilder.Entity<Task>().ToTable("Tasks");
            modelBuilder.Entity<Project>().ToTable("Projects");
            modelBuilder.Entity<Comment>().ToTable("Comments");

            base.OnModelCreating(modelBuilder);
        }
    }
}