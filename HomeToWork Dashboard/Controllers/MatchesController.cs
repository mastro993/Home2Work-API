using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeToWork_Dashboard.Controllers
{
    public class MatchesController : Controller
    {
        // GET: Matches
        public ActionResult Index()
        {

            ViewBag.Title = "Match - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 5;

            return View();
        }
    }
}