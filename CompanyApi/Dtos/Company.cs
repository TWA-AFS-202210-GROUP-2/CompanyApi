using Microsoft.AspNetCore.CookiePolicy;
using System;
using System.Collections.Generic;

namespace CompanyApi.Dtos
{
    public class Company
    {
        private static List<Employee> employees = new List<Employee>();

        public string? CompanyID { get; init; }
        public string? Name { get; set; }
        public List<Employee>? Employees { get; set; }
    }
}
