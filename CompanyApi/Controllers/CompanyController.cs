using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class PetController : Controller
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost("addNewCompany")]
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

        [HttpGet("getAllCompanies")]
        public ActionResult<List<Company>> GetAllPets()
        {
            return companies;
        }

        [HttpDelete("deleteAllCompanies")]
        public void DeleteAllPets()
        {
            companies.Clear();
        }

        [HttpGet("getByName")]
        public ActionResult<List<Company>> GetByPrice([FromQuery] string name)
        {
            return companies.FindAll(x => x.Name.Equals(name));
        }
    }
}