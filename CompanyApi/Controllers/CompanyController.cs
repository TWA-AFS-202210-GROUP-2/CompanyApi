using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CompanyController : Controller
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost("companies")]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companies.Exists(_ => _.Name.Equals(company.Name)))
            {
                return Conflict();
            }

            var thenewone = new Company(company.Name);
            companies.Add(thenewone);
            return Created("api/companies", thenewone);
        }

        [HttpDelete("companies")]
        public void DeleteAll()
        {
            companies.Clear();
        }

        [HttpGet("companies")]
        public ActionResult<List<Company>> GetAll([FromQuery] int? index, [FromQuery] int? pagesize)
        {
            if (index != null && pagesize != null)
            {
                int? start = (index - 1) * pagesize;
                int? end = index * pagesize;
                return Ok(companies.Where((item, index) => index >= start && index <= end).ToList());
            }

            return Ok(companies);
        }

        [HttpGet("companies/{id}")]
        public ActionResult<Company> GetById([FromRoute] string id)
        {
            return Ok(companies.Find(item => item._guid.Equals(id)));
        }

        [HttpPut("companies/{_guid}")]
        public ActionResult<Company> Put([FromRoute] string _guid, Company c)
        {
            companies.Find(item => item._guid.Equals(_guid)).Name = c.Name;
            return Ok(companies.Find(item => item.Name.Equals(c.Name)));
        }
        [HttpPost("companies/{_guid}/employees")]
        public ActionResult<Company> AddNewEmployee([FromRoute] string _guid, Company c)
        {
            companies.Find(item => item._guid.Equals(_guid)).Name = c.Name;
            return Ok(companies.Find(item => item.Name.Equals(c.Name)));
        }

    }
}
