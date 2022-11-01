using Microsoft.AspNetCore.CookiePolicy;
using System;

namespace CompanyApi
{
    public class Company
    {
        public Company(string name)
        {
            CompanyID = string.Empty;
            Name = name;
        }

        public string CompanyID { get; set; }
        public string Name { get; set; }
    }
}
