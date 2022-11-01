using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class PetController : Controller
    {
        private static List<Company> companies = new List<Company>();
        private static List<Employee> employees = new List<Employee>();

        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            var exist = companies.Exists(x => x.Name.Equals(company.Name));
            if (exist)
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpPost("{id}")]
        public ActionResult<Employee> AddNewEmployee(Employee employee, string id)
        {
            var exist = employees.Exists(x => x.Name.Equals(employee.Name));
            if (exist)
            {
                return new ConflictResult();
            }

            employee.CompanyID = Guid.NewGuid().ToString();
            employees.Add(employee);
            return new CreatedResult($"/companies/{id}/employees/", employee);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAll([FromQuery]int? pageSize, [FromQuery]int? pageIndex)
        {
            if (pageSize == null && pageIndex != null)
            {
                return companies.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }
            
            return companies;
        }

        [HttpDelete]
        public void DeleteAll()
        {
            companies.Clear();
        }

        [HttpDelete("{id}")]
        public ActionResult<List<Company>> DeleteCompany(string id)
        {
            companies.RemoveAll(x => x.CompanyID.Equals(id));
            return companies;
        }

        [HttpGet("{id}")]
        public ActionResult<List<Company>> GetCompany(string id)
        {
            return companies.FindAll(x => x.CompanyID.Equals(id));
        }
    }
}