using System;
using System.Collections.Generic;

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

    public override bool Equals(object? obj)
    {
        return obj is Company company &&
               Name == company.Name &&
               EqualityComparer<object>.Default.Equals(CompanyID, company.CompanyID);
    }
}
