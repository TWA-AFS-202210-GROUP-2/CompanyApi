using CompanyApi.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

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
            if (companies.Exists(item => item.Name == company.Name))
            {
                return new ConflictResult();
            }
            else
            {
                companies.Add(new Company 
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = company.Name,
                });
            }

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
