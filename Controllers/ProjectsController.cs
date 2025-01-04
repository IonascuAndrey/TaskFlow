using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Data;
using TaskFlow.Models;

namespace TaskFlow.Controllers {
    public class ProjectsController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProjectsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        ) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index() {
            if (TempData.ContainsKey("message")) {
                ViewBag.Message = TempData["message"].ToString();
            }
            var userId = _userManager.GetUserId(User);
            var userProjects = db.Projects
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId || p.ApplicationUsers.Any(u => u.Id == userId))
                .ToList();

            ViewBag.Projects = userProjects;
            return View();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult New() {
            var users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            ViewBag.Users = users;

            return View();
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Project model, List<string> SelectedUserIds) {
            try {
                // Get the current user's ID and assign it as the owner of the project
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.OwnerId = userId;

                // Initialize the ApplicationUsers collection if null
                model.ApplicationUsers ??= new List<ApplicationUser>();

                // Add the selected users to the project's ApplicationUsers collection
                if (SelectedUserIds != null && SelectedUserIds.Any()) {
                    foreach (var selectedUserId in SelectedUserIds) {
                        var user = db.Users.Find(selectedUserId);
                        if (user != null) {
                            model.ApplicationUsers.Add(user);
                        }
                    }
                }

                // Add the project to the database
                db.Projects.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                // Log or handle the exception as needed
                ModelState.AddModelError("", "An error occurred while saving the project.");
            }

            // If something goes wrong, repopulate the dropdown list for users
            ViewBag.Users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            return View(model);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id) {
            var project = db.Projects.Find(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != userId && !User.IsInRole("Admin")) {
                return Forbid(); // Nu are permisiunea
            }
            if (project != null) {
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost sters";
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id) {
            var project = db.Projects.Include(p => p.AppTasks).FirstOrDefault(p => p.Id == id);
            if (project == null) {
                return NotFound();
            }
            return View(project);
        }

        [HttpGet]
        public IActionResult Edit(int id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var project = db.Projects
                .Include(p => p.ApplicationUsers)
                .FirstOrDefault(p => p.Id == id);

            if (project == null) {
                return NotFound();
            }

            if (project.OwnerId != userId && !User.IsInRole("Admin")) {
                return Forbid(); // Nu are permisiunea
            }

            // Populate the list of users
            ViewBag.Users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project model, List<string> SelectedUserIds) {
            try {
                var userId_ = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var project = db.Projects
                    .Include(p => p.ApplicationUsers)
                    .FirstOrDefault(p => p.Id == model.Id);

                if (project == null) {
                    return NotFound();
                }

                if (project.OwnerId != userId_ && !User.IsInRole("Admin")) {
                    return Forbid();
                }

                // Update basic fields
                project.Title = model.Title;
                project.Description = model.Description;

                // Update associated users
                project.ApplicationUsers.Clear(); // Remove existing relationships

                if (SelectedUserIds != null && SelectedUserIds.Any()) {
                    foreach (var userId in SelectedUserIds) {
                        var user = db.Users.Find(userId);
                        if (user != null) {
                            project.ApplicationUsers.Add(user);
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                // Handle exceptions
                ModelState.AddModelError("", "An error occurred while updating the project.");
            }

            // If something goes wrong, repopulate the dropdown
            ViewBag.Users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            return View(model);
        }
    }
}
