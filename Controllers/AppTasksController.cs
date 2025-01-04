using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using AppTask = TaskFlow.Models.AppTask;

namespace TaskFlow.Controllers {
    public class AppTasksController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AppTasksController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        ) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult New(int projectId) {
            var project = db.Projects.Include(p => p.ApplicationUsers)
                                      .FirstOrDefault(p => p.Id == projectId);

            if (project == null) {
                return NotFound();
            }
            // Punem userii in ViewBag pt select
            ViewBag.Users = project.ApplicationUsers.Select(u => new SelectListItem {
                Value = u.Id,
                Text = u.UserName
            }).ToList();

            var task = new AppTask {
                ProjectId = projectId,
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now.AddDays(7)
            };

            return View(task);
        }

        [HttpPost]
        public IActionResult New(AppTask model, IFormFile Media) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors) {
                    Console.WriteLine(error.ErrorMessage);
                }

                foreach (var key in ModelState.Keys) {
                    var state = ModelState[key];
                    if (state.Errors.Any()) {
                        Console.WriteLine($"Field: {key}, Errors: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
                return View(model);
            }


            try {
                // Handle Media Upload
                if (Media != null && Media.Length > 0) {
                    // Generate a unique file name to prevent collisions
                    var uniqueFileName = $"{Guid.NewGuid()}_{Media.FileName}";
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Ensure the directory exists
                    if (!Directory.Exists(uploadsFolder)) {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Save the file to the uploads folder
                    using (var stream = new FileStream(filePath, FileMode.Create)) {
                        Media.CopyTo(stream);
                    }

                    // Save the file path to the model
                    model.Media = $"/uploads/{uniqueFileName}";
                }

                // Save the task in the database
                db.AppTasks.Add(model);
                db.SaveChanges();

                return RedirectToAction("Show", "Projects", new { id = model.ProjectId });
            }
            catch (Exception ex) {
                ModelState.AddModelError("", "An error occurred while saving the task.");
            }

            return View(model);
        }


    }
}
