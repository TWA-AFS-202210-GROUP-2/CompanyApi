using System;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string name)
        {
            CompanyID = Guid.NewGuid().ToString();
            Name = name;
        }

        public string CompanyID { get; set; }
        public string Name { get; set; }
    }
}
