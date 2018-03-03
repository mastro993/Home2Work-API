using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeToWork.Company;

namespace HomeToWork_API.Controllers
{
    public class CompanyController : ApiController
    {
        private readonly CompanyDao companyDao;

        public CompanyController()
        {
            companyDao = new CompanyDao();
        }


        [HttpGet]
        [Route("api/company/list")]
        public IHttpActionResult GetCompanyList()
        {
            var companies = companyDao.GetAll();
            return Ok(companies);
        }

        [HttpGet]
        [Route("api/company/{companyId:int}")]
        public IHttpActionResult GetCompanyById(int companyId)
        {
            var company = companyDao.GetById(companyId);
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
            company.Id = companyDao.Insert(company);

            if (company.Id == 0) return InternalServerError();

            return Ok(company);
        }
    }
}