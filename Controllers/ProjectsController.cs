using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index() {
            return View();
        }
        [HttpGet]
        public IActionResult New() {
            return View();
        }

        [HttpPost]
        public IActionResult New(Project model) {
            try {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.OwnerId = userId;
                db.Projects.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                return View(ex);
            }

            return View(model);
        }
    }
}
