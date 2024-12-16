namespace TaskFlow.Models {
    public class Project
{
    public int Id { get; set; }
    public string OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public ApplicationUser Owner { get; set; } // Foreign Key to ApplicationUser
    public ICollection<UserToProject> UserProjects { get; set; }
}

}
