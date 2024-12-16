using Microsoft.AspNetCore.Identity;
using TaskFlow.Models;
using Task = TaskFlow.Models.Task;

public class ApplicationUser : IdentityUser {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Navigation properties for related entities (e.g., tasks, projects)
    // A user can be assigned to multiple tasks
    public ICollection<Task>? Tasks { get; set; } // Name convention for M:M relation
    //A user can be a part of multiple projects
    public ICollection<Project>? Projects { get; set; } // Name convention for M:M relation
    //A user can post multiple comments
    public ICollection<Comment>? Comments { get; set; } // Name convention for 1:M relation
    //A user can create multiple projects
    public ICollection<Project>? OwnedProjects { get; set; }
}