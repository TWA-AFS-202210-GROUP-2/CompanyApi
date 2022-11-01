using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> CreateCompany(Company company)
        {
            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);

            return new CreatedResult("/companies/{company.CompanyID}", company);
        }
    }
}
