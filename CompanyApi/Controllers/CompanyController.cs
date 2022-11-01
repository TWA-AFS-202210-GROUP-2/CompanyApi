using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost("createdNewCompany")]
        public ActionResult<Company> CreateCompany(Company company)
        {
            var companyNameExsit = companies.Exists(x => x.Name.Equals(company.Name));
            if (companyNameExsit)
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);

            return new CreatedResult("/companies/{company.CompanyID}", company);
        }

        [HttpGet("getAllCompanies")]
        public ActionResult<List<Company>> GetAllCompany()
        {
            return companies;
        }

        [HttpDelete]
        public ActionResult DeleteAllCompanies()
        {
            companies.Clear();

            return new OkResult();
        }
    }
}
