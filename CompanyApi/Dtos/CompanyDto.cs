using System.Collections.Generic;

namespace CompanyApi.Dtos
{
    public class CompanyDto
    {
        public string? Name { get; set; }
        public List<Employee>? Employees { get; set; }
    }
}
