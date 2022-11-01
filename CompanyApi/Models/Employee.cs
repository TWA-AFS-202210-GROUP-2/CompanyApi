using System;

namespace CompanyApi.Models;

public class Employee
{
    public Employee(string name, int salary)
    {
        Name = name;
        Salary = salary;
        EmployeeId = Guid.NewGuid().ToString();
    }

    public string EmployeeId { get; set; }

    public string Name { get; }
    public int Salary { get; }

}