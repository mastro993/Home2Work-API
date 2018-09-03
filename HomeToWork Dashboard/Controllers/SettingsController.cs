using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeToWork_Dashboard.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult Index()
        {

            ViewBag.Title = "Impostazioni - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 6;

            return View();
        }
    }
}