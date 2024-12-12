using Microsoft.AspNetCore.Identity;
using TaskFlow.Models;

public class ApplicationUser : IdentityUser {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Navigation properties for related entities (e.g., tasks, projects)
    public ICollection<UserToTask> UserTasks { get; set; }
    public ICollection<UserToProject> UserProjects { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Project> OwnedProjects { get; set; }
}