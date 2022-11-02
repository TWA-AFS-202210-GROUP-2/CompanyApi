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

        [HttpPost("{id}/employees")]
        public ActionResult<Employee> AddNewEmployee(Employee employee, string id)
        {
            var exist = employees.Exists(x => x.Name.Equals(employee.Name));
            if (exist)
            {
                return new ConflictResult();
            }

            employee.EmployeeID = Guid.NewGuid().ToString();
            employees.Add(employee);
            return new CreatedResult($"/companies/{id}/employees/", employee);
        }

        [HttpGet("{id}/employees")]
        public ActionResult<List<Employee>> GetEmployes()
        {            
            return employees;
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAll([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
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
            employees.Clear();
        }

        [HttpDelete("{id}/employees/{eid}")]
        public ActionResult<List<Employee>> DeleteEmployee(string eid)
        {
            employees.RemoveAll(x => x.EmployeeID.Equals(eid));
            return employees;
        }

        [HttpDelete("{id}")]
        public ActionResult<List<Company>> DeleteCompany(string id)
        {
            companies.RemoveAll(x => x.CompanyID.Equals(id));
            return companies;
        }

        [HttpGet("{id}")]
        public ActionResult<Company> GetCompany(string id)
        {
            return companies.First(x => x.CompanyID.Equals(id));
        }

        [HttpGet("{id}/employees/{eid}")]
        public ActionResult<Employee> GetEmployees(string eid)
        {
            return employees.First(x => x.EmployeeID.Equals(eid));
        }

        [HttpPut("{id}")]
        public ActionResult<Company> PutCompany(string id, Company company)
        {
            var company1 = companies.First(x => x.CompanyID.Equals(id));
            company1.Name = company.Name;
            return company1;
        }

        [HttpPut("{id}/employees/{eid}")]
        public ActionResult<Employee> PutEmployees(string eid, Employee employee)
        {
            var employee1 = employees.First(x => x.EmployeeID.Equals(eid));
            employee1.Name = employee.Name;
            employee1.Salary = employee.Salary;
            return employee1;
        }
    }
}