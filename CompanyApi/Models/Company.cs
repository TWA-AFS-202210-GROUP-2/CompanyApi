using System;

namespace CompanyApi.Models;
public class Company
{
    private string name;

    public Company(string name)
    {
        this.name = name;
    }

    public string Name { get => name; set => name = value; }

    public override bool Equals(object? obj)
    {
        return obj is Company company &&
               name == company.name;
    }
}
