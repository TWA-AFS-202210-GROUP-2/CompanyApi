using System.Collections.Generic;

namespace CompanyApi.Dto
{
    public class Company
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public List<string>? EmplyeeId { get; set; }
    }
}
