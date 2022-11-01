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
        public ActionResult<List<Company>> GetAllCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                return companies.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }

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

        [HttpPut("{companyID}")]
        //should return companies?
        public ActionResult<List<Company>> UpdateCompany(Company company, [FromRoute] string companyID)
        {
            var foundCompany = companies.FirstOrDefault(company => company.CompanyID == companyID);
            if (foundCompany == null)
            {
                return new NotFoundResult();
            }

            companies.Remove(foundCompany);
            companies.Add(company);
            return Ok(companies);
        }

        [HttpPost("{companyID}/employees")]
        //should return employee or company?
        public ActionResult<Employee> CreateEmployee(Employee employee, [FromRoute] string companyID)
        {
            employee.EmployeeID = Guid.NewGuid().ToString();
            var foundCompany = companies.FirstOrDefault(company => company.CompanyID == companyID);
            if (foundCompany == null)
            {
                return new NotFoundResult();
            }

            foundCompany.Employees.Add(employee);
            return Created($"/companies/{companyID}/employees", employee);
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }
    }
}
