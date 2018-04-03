using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace HomeToWork_API.Controllers
{
    public class CompanyController : ApiController
    {
        private readonly ICompanyRepository _companyRepo;

        public CompanyController()
        {
            _companyRepo = new CompanyRepository();
        }

        [HttpGet]
        [Route("api/company/list")]
        public IHttpActionResult GetCompanyList()
        {
            var companies = _companyRepo.GetAll();
            return Ok(companies);
        }

        [HttpGet]
        [Route("api/company/{companyId:int}")]
        public IHttpActionResult GetCompanyById(int companyId)
        {
            var company = _companyRepo.GetById(companyId);
            return Ok(company);
        }

        [HttpGet]
        [Route("api/company/{companyId:int}/profile")]
        public IHttpActionResult GetCompanyProfile(int companyId)
        {
            // TODO profilo azienda
            return NotFound();
        }

        [HttpPost]
        [Route("api/company/insert")]
        public IHttpActionResult PostNewCompany(Company company)
        {
            return NotFound();
        }
    }
}