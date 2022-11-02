using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace CompanyApiTest.Controllers
{
    public class Company
    {
        /*private string name;*/

        public Company(string name)
        {
            CompanyID = string.Empty;
            Employees = new List<Employee>();
            Name = name;
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
        public List<Employee> Employees { get; set; }

        public override bool Equals(object? obj)
        {
            var company = obj as Company;
            return company != null &&
                Name.Equals(company.Name) &&
                CompanyID.Equals(company.CompanyID);
        }
    }
}