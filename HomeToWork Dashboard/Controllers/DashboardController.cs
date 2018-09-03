using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using data.Repositories;

namespace HomeToWork_API.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 0;

            return View();
        }

    }
}