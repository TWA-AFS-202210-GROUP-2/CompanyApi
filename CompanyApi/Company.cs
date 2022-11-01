using System;

namespace CompanyApiTest.Controllers
{
    public class Company
    {
        /*private string name;*/

        public Company(string name)
        {
            CompanyID = string.Empty;
            Name = name;
        }

        public string Name { get; set; }
        public string CompanyID { get; set; }
    }
}