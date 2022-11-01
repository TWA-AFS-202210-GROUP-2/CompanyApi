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
        [HttpPost("company")]
        public Company CreateCompany(Company company)
        {
            companies.Add(company);
            return company;
        }
    }
}
