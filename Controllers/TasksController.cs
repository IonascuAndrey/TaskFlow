using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;

namespace TaskFlow.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TasksController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(int projectId)
        {
            var project = db.Projects.FirstOrDefault(p => p.Id == projectId);
            ViewBag.Project = project;
            var tasks = db.Tasks.Where(t => t.ProjectId == projectId)
                          .Include(t => t.Project)
                          .OrderBy(t => t.DateStart)
                          .ToList();
            ViewBag.Tasks = tasks;
            return View();
        }
        [HttpGet]
        public IActionResult New(int projectId)
        {
            var project = db.Projects.FirstOrDefault(p => p.Id == projectId);
            ViewBag.Project = project;
            return View();
        }
        [HttpPost]
        public IActionResult New(Models.Task task)
        {
            //try
            //{

            //    db.Tasks.Add(task);
            //    db.SaveChanges();
            //    TempData["message"] = "Task-ul a fost creat cu succes!";
            //    return RedirectToAction("Index", new { projectId = task.ProjectId });
            //}
            //catch (Exception ex){
            //    Console.WriteLine(ex.Message);
            //}
            ViewBag.Task = task;


            return View(task);

        }
        [NonAction]
        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return db.Users.ToList();
        }
    }
}