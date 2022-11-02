using CompanyApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> CreateCompany(CompanyDto companyDto)
        {
            var companyNameExsit = companies.Exists(x => x.Name.Equals(companyDto.Name));
            if (companyNameExsit)
            {
                return new ConflictResult();
            }

            var company = new Company
            {
                CompanyID = Guid.NewGuid().ToString(),
                Name = companyDto.Name,
            };
            companies.Add(company);

            return new CreatedResult("/companies/{company.CompanyID}", company);
        }

        [HttpPost("{companyID}/employees")]
        public ActionResult<Employee> AddNewEmployee([FromRoute] string companyID, [FromBody] EmployeeDto employeeDto)
        {
            var company = companies.FirstOrDefault(company => company.CompanyID.Equals(companyID));

            var employee = new Employee
            {
                Name = employeeDto.Name,
                ID = Guid.NewGuid().ToString(),
            };
            company.Employees.Add(employee);

            return employee;
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetCompanyByID([FromRoute] string companyID)
        {
            return companies.FirstOrDefault(company => company.CompanyID.Equals(companyID));
        }

        [HttpGet]
        public ActionResult<List<Company>> GetCompaniesByIndex([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageSize != null)
            {
                return companies.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }

            return companies;
        }

        [HttpGet("{companyID}/employees")]
        public ActionResult<List<Employee>> GetEmloyessByCompanyID([FromRoute] string companyID)
        {
            return companies.FirstOrDefault(company => company.CompanyID.Equals(companyID)).Employees;
        }

        [HttpPut("{companyID}")]
        public ActionResult<Company> UpdateCompany([FromRoute] string companyID, [FromBody] CompanyDto companyDto)
        {
            try
            {
                var oldCompany = companies.FirstOrDefault(_ => _.CompanyID.Equals(companyID));
                oldCompany.Name = companyDto.Name;
            }
            catch (Exception e)
            {
                throw new Exception("no found");
            }

            return companies.FirstOrDefault(company => company.CompanyID.Equals(companyID));
        }

        [HttpPut("{companyID}/{employeeID}")]
        public ActionResult<Employee> UpdateEmployeeInfoByCompanyAndID([FromRoute] string companyID, [FromRoute] string employeeID, [FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var company = companies.FirstOrDefault(_ => _.CompanyID.Equals(companyID));
                var selectedEmployee = company.Employees.FirstOrDefault(employee => employee.ID.Equals(employeeID));
                selectedEmployee.Name = employeeDto.Name;
            }
            catch (Exception e)
            {
                throw new Exception("no found");
            }

            return companies.FirstOrDefault(_ => _.CompanyID.Equals(companyID))
                .Employees.FirstOrDefault(employee => employee.ID.Equals(employeeID));
        }

        [HttpDelete]
        public ActionResult DeleteAllCompanies()
        {
            companies.Clear();

            return new OkResult();
        }

        [HttpDelete("{companyID}/{employeeID}")]
        public ActionResult DeleteEmployFromCompany([FromRoute] string companyID, [FromRoute] string employeeID)
        {
            var company = companies.FirstOrDefault(_ => _.CompanyID.Equals(companyID));
            var selectedEmployee = company.Employees.FirstOrDefault(employee => employee.ID.Equals(employeeID));
            company.Employees.Remove(selectedEmployee);

            return new OkResult();
        }

        [HttpDelete("{companyID}")]
        public ActionResult DeleteCompanyByID([FromRoute] string companyID)
        {
            var company = companies.FirstOrDefault(_ => _.CompanyID.Equals(companyID));
            company.Employees.Clear();
            companies.Remove(company);

            return new OkResult();
        }
    }
}
