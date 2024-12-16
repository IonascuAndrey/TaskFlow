using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaskFlow.Models {
    public class Project{
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "The title of the project is mandatory!")]
        [StringLength(20, ErrorMessage = "The title must be shorter than 20 characters")]
        [MinLength(5, ErrorMessage = "The title must be at least 5 characters long")]
        public string Title { get; set; }
        [StringLength(200, ErrorMessage = "The description must be shorter than 200 characters")]
        public string Description { get; set; }
        // FK
        public string? OwnerId { get; set; }
        // Virtual property
        public virtual ApplicationUser? Owner { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; } // Name convention for M:M relation
    }

}
