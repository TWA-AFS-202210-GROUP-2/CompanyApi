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
using System.Net;

namespace PetApiTest.ControllerTest;
public class CompanyController
{
    [Fact]
    public async Task Should_return_add_company_success_when_add_company()
    {
        // given
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        await httpClient.DeleteAsync("/api/deleteAllCompanies");
        var company = new Company(name: "SLB");
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/addNewCompany", stringContent);
        // when
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        // then
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
        Assert.Equal(company.Name, saved.Name);
        Assert.NotEmpty((System.Collections.IEnumerable)saved.CompanyID);
    }

    [Fact]
    public async Task Should_return_failed_success_when_add_same_company()
    {
        // given
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        await httpClient.DeleteAsync("/api/deleteAllCompanies");
        var company = new Company(name: "SLB");
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        // when
        await httpClient.PostAsync("api/addNewCompany", stringContent);
        var response = await httpClient.PostAsync("api/addNewCompany", stringContent);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Should_return_company_success_when_searched_by_name()
    {
        // given
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        await httpClient.DeleteAsync("/api/deleteAllCompanies");
        var company = new Company(name: "SLB");
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        await httpClient.PostAsync("api/addNewCompany", stringContent);
        var response = await httpClient.GetAsync($"api/getByName?name={company.Name}");
        // when
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // then
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
        Assert.Equal(company.Name, saved.Name);
        Assert.NotEmpty((System.Collections.IEnumerable)saved.CompanyID);
    }
}