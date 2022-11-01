using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class PetController : Controller
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost("addNewCompany")]
        public Company AddNewPet(Company company)
        {
            companies.Add(company);
            return company;
        }
    }
}