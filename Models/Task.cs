using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskFlow.Models {
    public class Task {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The title of the task is mandatory!")]
        [StringLength(20, ErrorMessage = "The title must be shorter than 20 characters")]
        [MinLength(5, ErrorMessage = "The title must be at least 5 characters long")]
        public string Title { get; set; }
        [StringLength(200, ErrorMessage = "The description must be shorter than 200 characters")]
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Media { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
