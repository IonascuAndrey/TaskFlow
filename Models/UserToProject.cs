namespace TaskFlow.Models {
    public class UserToProject {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        public ApplicationUser? User { get; set; }
        public Project? Project { get; set; }
    }

}
