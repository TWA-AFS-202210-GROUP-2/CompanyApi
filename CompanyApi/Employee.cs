using System;
using System.Drawing;
using System.Xml.Linq;

namespace CompanyApiTest.Controllers
{
    public class Employee
    {
        /*private string name;*/

        public Employee(string name, int salary)
        {
            EmployeeID = string.Empty;
            Name = name;
            Salary = salary;
        }

        public string Name { get; set; }
        public string EmployeeID { get; set; }
        public int Salary { get; set; }

        public override bool Equals(object? obj)
        {
            var employee = obj as Employee;
            return employee != null &&
                Name.Equals(employee.Name) &&
                EmployeeID.Equals(employee.EmployeeID) &&
                Salary.Equals(employee.Salary);
        }
    }
}