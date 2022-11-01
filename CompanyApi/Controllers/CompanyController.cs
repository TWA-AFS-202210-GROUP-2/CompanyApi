using CompanyApi.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CompanyApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        public static List<Company> companies = new List<Company>();

        [HttpPost("companies")]
        public ActionResult<Company> CreateCompany([FromBody]Company company)
        {
            if (companies.Exists(item => item.Name == company.Name))
            {
                return new ConflictResult();
            }
            else
            {
                var newCompany = new Company
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = company.Name,
                };
                companies.Add(newCompany);
                return new CreatedResult($"companies/{newCompany.Id}", newCompany);
            }            
        }

        [HttpGet("companies")]
        public ActionResult<List<Company>> GetCompanies([FromQuery]int? pageSize, [FromQuery]int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            { 
                return companies.GetRange((pageIndex.Value - 1) * pageSize.Value, pageSize.Value);
            }

            return new ObjectResult(companies);
        }

        [HttpGet("companies/{id}")]
        public ActionResult<Company> GetCompanyById([FromRoute] string id)
        {
            if (companies.Exists(item => item.Id == id))
            {
                return companies.Find(item => item.Id == id);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("companies")]
        public IActionResult DeleteAllCompanies()
        {
            companies.Clear();
            return NoContent();
        }

        [HttpPatch("campnies/{id}")]
        public ActionResult<Company> UpdateCompany([FromBody] Company newCompany)
        {
            if (companies.Exists(item => item.Id == newCompany.Id))
            {
                var campany = companies.Find(item => item.Id == newCompany.Id);
                campany.Name = newCompany.Name;
                return campany;
            }
            else
            {
                return NotFound();
            }
        }
    }
}
