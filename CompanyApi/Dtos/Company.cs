using Microsoft.AspNetCore.CookiePolicy;
using System;
using System.Collections.Generic;

namespace CompanyApi.Dtos
{
    public class Company
    {
        public string CompanyID { get; init; }
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
