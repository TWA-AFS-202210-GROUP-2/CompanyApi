﻿using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            var companyNameExists = companies.Exists(x => x.Name == company.Name);
            if (companyNameExists)
            {
                return new ConflictResult();
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpGet]
        public List<Company> ObtainAllCompany()
        {
            return companies;
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet("/{id}")]
        public Company ObtainExistCompany([FromRoute] string id)
        {
            var company = companies.Find(x => x.CompanyID == id);
            return company;
        }

        [HttpGet]
        public List<Company> GetCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if ( pageSize != null && pageIndex != null)
            
            {
                return companies.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }

            return companies;
        }
    }
}
