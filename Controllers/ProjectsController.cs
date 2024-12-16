using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Controllers {
    public class ProjectsController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
