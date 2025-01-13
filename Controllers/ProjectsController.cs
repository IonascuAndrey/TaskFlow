using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis;
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

            var allProjects = db.Projects.Include(p => p.ApplicationUsers).ToList();

            var projectsAsOwner = allProjects.Where(p => p.OwnerId == userId).ToList();

            var projectsNotOwner = allProjects
		.Where(p => p.OwnerId != userId && p.ApplicationUsers.Any(u => u.Id == userId))
		.ToList();

			ViewBag.ProjectsAsOwner = projectsAsOwner;
            ViewBag.ProjectsNotOwner = projectsNotOwner;


            var userProjects = db.Projects
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == userId || p.ApplicationUsers.Any(u => u.Id == userId))
                .ToList();

            ViewBag.Projects = userProjects;
            return View(allProjects);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> New()
        {
            var users = await db.Users.ToListAsync();

            var nonAdmins = new List<SelectListItem>();

            foreach (var user in users)
            {
                if (!(await _userManager.IsInRoleAsync(user, "Admin")))
                {
                    nonAdmins.Add(new SelectListItem
                    {
                        Value = user.Id,
                        Text = user.UserName
                    });
                }
            }

            ViewBag.Users = nonAdmins;


            return View(); 
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public async Task<IActionResult> New(TaskFlow.Models.Project model, List<string> SelectedUserIds) {
            try {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.OwnerId = userId;

                model.ApplicationUsers ??= new List<ApplicationUser>();

                if (SelectedUserIds != null && SelectedUserIds.Any()) {
                    foreach (var selectedUserId in SelectedUserIds) {
                        var user = db.Users.Find(selectedUserId);
                        if (user != null) {
                            model.ApplicationUsers.Add(user);
                        }
                    }
                }
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole != null)
                {
                    var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name);
                    foreach (var admin in adminUsers)
                    {
                        if (!model.ApplicationUsers.Any(u => u.Id == admin.Id)) 
                        {
                            model.ApplicationUsers.Add(admin);
                        }
                    }
                }
                TempData["message"] = "Proiectul a fost creat cu succes!";
                db.Projects.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                ModelState.AddModelError("", "An error occurred while saving the project.");
            }

            ViewBag.Users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            return View(model);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Delete(int id) {
            var project = db.Projects
                .Include(p => p.AppTasks)
                .ThenInclude(t => t.Comments) // Include comments for each task
                .FirstOrDefault(p => p.Id == id);

            if (project == null) {
                return NotFound();
            }

            foreach (var task in project.AppTasks) {
                if (task.Comments != null) {
                    db.Comments.RemoveRange(task.Comments);
                }
            }

            if (project.AppTasks != null) {
                db.AppTasks.RemoveRange(project.AppTasks);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != userId && !User.IsInRole("Admin")) {
                return Forbid(); 
            }
            if (project != null) {
                var appTasks = db.AppTasks.Where(t => t.ProjectId == id).ToList();
                db.AppTasks.RemoveRange(appTasks);
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost sters";
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Show(int id) {
            var project = db.Projects.Include(p => p.AppTasks).Include(p => p.ApplicationUsers).FirstOrDefault(p => p.Id == id);

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
                return Forbid(); 
            }

            ViewBag.Users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(TaskFlow.Models.Project model, List<string> SelectedUserIds) {
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

                project.Title = model.Title;
                project.Description = model.Description;

                project.ApplicationUsers.Clear(); 

                if (SelectedUserIds != null && SelectedUserIds.Any()) {
                    foreach (var userId in SelectedUserIds) {
                        var user = db.Users.Find(userId);
                        if (user != null) {
                            project.ApplicationUsers.Add(user);
                        }
                    }
                }
                TempData["message"] = "Proiectul a fost editat cu succes!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                ModelState.AddModelError("", "An error occurred while updating the project.");
            }

            ViewBag.Users = db.Users.Select(user => new SelectListItem {
                Value = user.Id,
                Text = user.UserName
            }).ToList();

            return View(model);
        }
		

	}
}
