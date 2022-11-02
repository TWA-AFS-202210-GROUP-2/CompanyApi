namespace CompanyApi.Dtos
{
    public class Employee
    {
        public string? Name { get; set; }
        public string? ID { get; init; }
        public int? Salary { get; set; }
    }
}