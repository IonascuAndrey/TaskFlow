namespace TaskFlow.Models {
    public class Comment {
        public int Id { get; set; }
        public string Content { get; set; }
        //FK
        public int? TaskId { get; set; }
        // FK
        public string? UserId { get; set; }
        public DateTime DateAdd { get; set; }
        public Task? Task { get; set; } // Virtual Property
        public virtual ApplicationUser? User { get; set; } //Virtual property
    }
}
