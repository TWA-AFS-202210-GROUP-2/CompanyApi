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
            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return Created("/companies", value: company);
        }

        /*[HttpGet]*/
    }
}
