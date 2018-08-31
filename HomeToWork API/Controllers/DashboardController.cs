using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Users()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 1;

            return View();
        }

        public ActionResult Companies()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 2;

            return View();
        }

        public ActionResult Locations()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 3;

            return View();
        }

        public ActionResult Routines()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 4;

            return View();
        }

        public ActionResult Matches()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 5;

            return View();
        }

        public ActionResult Settings()
        {
            ViewBag.Title = "Dashboard";
            ViewBag.SelectedNavbar = 6;

            return View();
        }
    }
}