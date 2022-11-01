using CompanyApiTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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

        [HttpDelete("deleteAll")]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }
    }
}
