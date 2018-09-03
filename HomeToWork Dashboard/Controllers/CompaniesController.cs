using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using data.Repositories;

namespace HomeToWork_Dashboard.Controllers
{
    public class CompaniesController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Aziende - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 2;

            var companyRepo = new CompanyRepository();

            var companies = companyRepo.GetAll();
            ViewBag.Companies = companies;

            return View(companies);
        }

        public ActionResult Details(long id)
        {
            ViewBag.Title = "Aziende - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 2;

            var companyRepo = new CompanyRepository();

            var company = companyRepo.GetById(id);
            ViewBag.Company = company;

            return View(company);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Aggiungi Azienda - Dashboard Home2Work";
            ViewBag.SelectedNavbar = 2;

            return View();
        }

    }
}