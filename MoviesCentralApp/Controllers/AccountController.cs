using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesCentralApp.Models;

namespace MoviesCentralApp.Controllers
{
    public class AccountController : Controller
    {
        MoviesCentralDBContext dbContext = new MoviesCentralDBContext();
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        
        public IActionResult Register(User user) {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(x => x.Email == user.Email))
                {
                    ViewBag.Message = "Email already registered";
                    return View();
                }else if (dbContext.Users.Any(x => x.Username == user.Username))
                {
                    ViewBag.Message = "Username already registered";
                    return View();
                }
                else
                {
                    
                    user.Role = "user";
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                    return View("~/Views/Account/Login.cshtml");
                }
                
            }
            return View();
            
        }


        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Remove("UserId");
            return View();
        }

        [HttpPost]
        public IActionResult Login(MyLogin myLogin)
        {
            var querry = dbContext.Users.SingleOrDefault(m => m.Email == myLogin.Email && m.Password == myLogin.Password);

            

            if(querry != null)
            {
                HttpContext.Session.SetInt32("UserId", querry.Userid);


                TempData["Message"] = "Login successful";

                if (querry.Role == "admin")
                {
                    return RedirectToAction("AdminLoginView", "Admin");
                }
                else
                {
                    return View("~/Views/LoginView.cshtml");
                }

                
            }
            else
            {

                TempData["Message"] = "Login failed";
                return View("~/Views/Account/Login.cshtml");
            }

            
        }

    }

    
}
