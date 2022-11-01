using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> CreateCompany(Company company)
        {
            var companyNameExsit = companies.Exists(x => x.Name.Equals(company.Name));
            if (companyNameExsit)
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
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
                return companies
                    .Skip((pageSize.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return companies;
        }

        [HttpDelete]
        public ActionResult DeleteAllCompanies()
        {
            companies.Clear();

            return new OkResult();
        }
    }
}
