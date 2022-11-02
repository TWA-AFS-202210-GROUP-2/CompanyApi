using System;
using System.Collections.Generic;

namespace CompanyApi.Models;

public class Company
{
    public List<Employee> employees = new List<Employee>();
    public Company(string name)
    {
        Name = name;
        _guid = Guid.NewGuid().ToString();
    }

    public string Name { get; set; }
    public string? _guid { get; set; }
}