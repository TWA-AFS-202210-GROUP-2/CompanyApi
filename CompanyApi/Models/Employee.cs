using System;
using System.Collections.Generic;

namespace CompanyApi.Models;
public class Employee
{
    public Employee(string name, int salary)
    {
        EmployeeID = string.Empty;
        Name = name;
        Salary = salary;
    }

    public string Name { get; set; }
    public int Salary { get; set; }
    public object EmployeeID { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Employee employee &&
               Name == employee.Name &&
               Salary == employee.Salary &&
               EqualityComparer<object>.Default.Equals(EmployeeID, employee.EmployeeID);
    }
}