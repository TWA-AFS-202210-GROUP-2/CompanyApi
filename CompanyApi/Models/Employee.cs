using System;
using System.Collections.Generic;

namespace CompanyApi.Models;
public class Employee
{
    public Employee(string name, int salary)
    {
        CompanyID = string.Empty;
        Name = name;
        Salary = salary;
    }

    public string Name { get; set; }
    public int Salary { get; set; }
    public object CompanyID { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Employee employee &&
               Name == employee.Name &&
               Salary == employee.Salary &&
               EqualityComparer<object>.Default.Equals(CompanyID, employee.CompanyID);
    }
}