using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeToWork_Dashboard.Controllers
{
    public class RoutinesController : Controller
    {
        // GET: Routines
        public ActionResult Index()
        {
            ViewBag.Title = "Routine - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 4;

            return View();
        }
    }
}