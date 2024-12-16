using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Controllers {
    public class TasksController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
