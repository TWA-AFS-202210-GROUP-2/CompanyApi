using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class PetController : Controller
    {
        private static List<Company> companies = new List<Company>();

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

        [HttpGet]
        public ActionResult<List<Company>> GetAll()
        {
            return companies;
        }

        [HttpDelete]
        public void DeleteAll()
        {
            companies.Clear();
        }

        [HttpGet("{id}")]
        public ActionResult<List<Company>> GetCompany(string id)
        {
            return companies.FindAll(x => x.CompanyID.Equals(id));
        }
    }
}