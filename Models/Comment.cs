namespace TaskFlow.Models {
    public class Comment {
        public int Id { get; set; }
        public string Content { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public DateTime DateAdd { get; set; }

        public Task Task { get; set; } // Foreign Key to Task
        public ApplicationUser User { get; set; } // Foreign Key to ApplicationUser
    }
}
