using CompanyApiTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            var companyNameExists = companies.Exists(companyToCompare => companyToCompare.Name == company.Name);
            if (companyNameExists) 
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return Created("/companies", value: company);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompanies()
        {
            return Ok(companies);
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetCompanyById([FromRoute] string companyID)
        {
            var foundCompany = companies.FirstOrDefault(company => company.CompanyID == companyID);
            if (foundCompany == null)
            {
                return new NotFoundResult();
            }

            return Ok(foundCompany);
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }
    }
}
