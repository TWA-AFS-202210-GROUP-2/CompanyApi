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

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompany()
        {
            return companies;
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetAllCompany([FromRoute] string companyID)
        {
            return companies.FirstOrDefault(company => company.CompanyID.Equals(companyID));
        }

        [HttpGet]
        public ActionResult<List<Company>> GetCompaniesByIndex([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageSize != null)
            {
                return companies.GetRange((pageSize.Value - 1) * pageSize.Value, pageSize.Value);
            }

            return companies;
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

        [HttpDelete]
        public ActionResult DeleteAllCompanies()
        {
            companies.Clear();

            return new OkResult();
        }
    }
}
