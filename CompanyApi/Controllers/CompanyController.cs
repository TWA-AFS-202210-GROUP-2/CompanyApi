using CompanyApi.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public static List<Company> companies = new List<Company>();

        [HttpPost("companies")]
        public ActionResult<Company> CreateCompany(Company company)
        {
            companies.Add(company);
            return new CreatedResult($"companies/{company.Id}", company);
        }

        [HttpGet("companies")]
        public ActionResult<List<Company>> GetCompanies()
        {
            return new ObjectResult(companies);
        }

        [HttpDelete("companies")]
        public IActionResult DeleteAllCompanies()
        {
            companies.Clear();
            return NoContent();
        }
    }
}
