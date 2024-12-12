using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Models {
    public class Task {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Media { get; set; }

        public ICollection<UserToTask> UserTasks { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
