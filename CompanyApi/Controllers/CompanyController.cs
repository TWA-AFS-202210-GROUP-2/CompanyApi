using System;
using System.Collections.Generic;
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
        public ActionResult<List<Company>> GetAll()
        {
            Console.WriteLine(companies.Count.ToString());
            return Ok(companies);
        }

        [HttpGet("companies/{_guid}")]
        public ActionResult<Company> GetById([FromRoute] string _guid)
        {
            return Ok(companies.Find(item => item._guid.Equals(_guid)));
        }
        [HttpGet("companies")]
        public ActionResult<Company> GetByIndexAndPageSize([FromQuery] int index, [FromQuery] int pagesize)
        {
            return Ok(companies.Find(item => item._guid.Equals(_guid)));
        }
    }
}
