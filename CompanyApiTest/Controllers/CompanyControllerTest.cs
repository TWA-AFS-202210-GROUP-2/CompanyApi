using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using CompanyApi.Controllers;
using CompanyApi.Models;
using Xunit;
namespace PetApiTest.ControllerTest;
public class CompanyController
{
    [Fact]
    public async Task Should_return_add_company_success_when_add_company()
    {
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        var company = new Company(name: "SLB");
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/addNewCompany", stringContent);
        response.EnsureSuccessStatusCode();
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
        Assert.Equal(company, saved);
    }
}