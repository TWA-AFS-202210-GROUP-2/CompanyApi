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
        public static List<Employee> employees = new List<Employee>();
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

        [HttpDelete("employees")]
        public IActionResult DeleteAllEmployees()
        {
            employees.Clear();
            return NoContent();
        }

        [HttpPatch("companies/{id}")]
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

        [HttpPost("companies/{id}/employees")]
        public ActionResult<Employee> CreateEmployee([FromBody] Employee employee, [FromRoute] string id)
        {
            if (companies.Exists(item => item.Id == id))
            {
                if (employees.Exists(item => item.Name == employee.Name))
                {
                    return BadRequest();
                }
                else 
                {
                    var company = companies.Find(item => item.Id == id);
                    var newEmployee = new Employee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = employee.Name,
                        CompanyId = company.Id,
                    };
                    employees.Add(newEmployee);
                    company.EmplyeeId.Add(newEmployee.Id);
                    return new CreatedResult($"companies/{company.Id}/employees/{newEmployee.Id}", newEmployee);
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
