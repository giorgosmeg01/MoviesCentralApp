using Microsoft.AspNetCore.Mvc;

namespace MoviesCentralApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminLoginView()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;

            return View();
        }
    }
}
