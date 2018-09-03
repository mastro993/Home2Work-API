using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using data.Repositories;

namespace HomeToWork_Dashboard.Controllers
{
    public class UsersController : Controller
    {
        // GET: User
        public ActionResult Index()
        {

            ViewBag.Title = "Utenti - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 1;

            var userRepo = new UserRepository();

            var users = userRepo.GetAll();
            ViewBag.Users = users;

            return View(users);

        }
    }
}