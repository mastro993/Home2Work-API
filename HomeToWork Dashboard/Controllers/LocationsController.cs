using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeToWork_Dashboard.Controllers
{
    public class LocationsController : Controller
    {
        // GET: Locations
        public ActionResult Index()
        {

            ViewBag.Title = "Posizioni - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 3;

            return View();
        }
    }
}