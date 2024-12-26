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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            }

            var projects = db.Projects
                             .Include(p => p.Owner)
                             .OrderBy(p => p.Title)
                             .ToList();

            ViewBag.Projects = projects;
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
                TempData["message"] = "Proiectul a fost adaugat";

                //var user = await _userManager.FindByIdAsync(userId);
                //if (user != null)
                //{
                //    if (! await _userManager.IsInRoleAsync(user, "Organizator"))
                //    {
                //        await _userManager.AddToRoleAsync(user, "Organizator");
                //    }
                //}


                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var projectt = db.Projects.Find(id);
            if (projectt != null)
            {
                db.Projects.Remove(projectt);
                db.SaveChanges();
                TempData["message"] = "Proiectul a fost sters";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Show(int id)
        {
            var project = db.Projects.Include(p => p.Tasks).FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

    }
}
