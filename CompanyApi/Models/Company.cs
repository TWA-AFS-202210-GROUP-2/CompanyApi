using System;

namespace CompanyApi.Models;

public class Company
{
   
    public Company(string name)
    {
        Name = name;
        _guid = Guid.NewGuid().ToString();
    }
    public string Name { get; set; }
    public string? _guid { get; set; }

}