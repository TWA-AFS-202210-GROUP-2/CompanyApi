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
        await httpClient.DeleteAsync("/companies");
        var company = new Company(name: "SLB");
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("companies", stringContent);
        // when
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        // then
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
        Assert.Equal(company.Name, saved.Name);
        Assert.NotEmpty((System.Collections.IEnumerable)saved.CompanyID);
    }

    [Fact]
    public async Task Should_return_failed_when_add_same_company()
    {
        // given
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        await httpClient.DeleteAsync("/companies");
        var company = new Company(name: "SLB");
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        // when
        await httpClient.PostAsync("companies", stringContent);
        var response = await httpClient.PostAsync("companies", stringContent);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Should_return_all_companies_when_searched_all()
    {
        // given
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        await httpClient.DeleteAsync("/companies");
        var company1 = await AddCompanyAsync("SLB", httpClient);
        var company2 = await AddCompanyAsync("ABC", httpClient);
        var response = await httpClient.GetAsync($"companies/");
        // when
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // then
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<List<Company>>(readAsStringAsync);
        Assert.Equal(company1, saved[0]);
        Assert.Equal(company2, saved[1]);
    }

    [Fact]
    public async Task Should_return_company_when_searched_by_name()
    {
        // given
        var application = new WebApplicationFactory<Program>();
        var httpClient = application.CreateClient();
        await httpClient.DeleteAsync("/companies");
        var company1 = await AddCompanyAsync("SLB", httpClient);
        var company2 = await AddCompanyAsync("ABC", httpClient);
        var response = await httpClient.GetAsync($"companies/{company1.CompanyID}");
        // when
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // then
        var readAsStringAsync = await response.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<List<Company>>(readAsStringAsync);
        Assert.Equal(company1, saved[0]);
        }

    private async Task<Company> AddCompanyAsync(string name, HttpClient httpClient)
    {
        var company = new Company(name: name);
        var serializeObject = JsonConvert.SerializeObject(company);
        var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");
        var company1 = await httpClient.PostAsync("companies", stringContent);
        var readAsStringAsync = await company1.Content.ReadAsStringAsync();
        var saved = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
        return saved;
    }
}