namespace TaskFlow.Models {
    public class UserToTask {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }

        public ApplicationUser? User { get; set; }
        public Task? Task { get; set; }
    }

}
