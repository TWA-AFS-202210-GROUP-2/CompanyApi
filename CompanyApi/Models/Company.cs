using System;

namespace CompanyApi.Models;
public class Company
{
    public Company(string name)
    {
        CompanyID = string.Empty;
        Name = name;
    }

    public string Name { get; set; }
    public object CompanyID { get; set; }
}
