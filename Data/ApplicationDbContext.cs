using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TaskFlow.Models;
using Task = TaskFlow.Models.Task;

namespace TaskFlow.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Configure the foreign keys
            // ONE TO MANY
            // Project N:1 Owner
            modelBuilder.Entity<Project>()
                 .HasOne<ApplicationUser>(a => a.Owner)
                 .WithMany(c => c.OwnedProjects)
                 .HasForeignKey(a => a.OwnerId);
            // Comment N:1 Task done with name convention
            // Comment N:1 Owner
            modelBuilder.Entity<Comment>()
                 .HasOne<ApplicationUser>(a => a.User)
                 .WithMany(c => c.Comments)
                 .HasForeignKey(a => a.UserId);
            // MANY TO MANY
            // Users - Tasks done with name convention
            // Users - Projects (members) done with name convention

            base.OnModelCreating(modelBuilder);
        }
    }
}
