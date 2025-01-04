using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Controllers {
    public class AdministrationController : Controller {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdministrationController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        ) {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index() {
            if (TempData.ContainsKey("message")) {
                ViewBag.Message = TempData["message"].ToString();
            }
            var userId = _userManager.GetUserId(User);
            var userProjects = db.Projects
                .ToList();

            ViewBag.Projects = userProjects;
            return View();
        }
    }
}
