using CompanyApi.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace CompanyApiTest
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_new_company_successfullyAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            var company = new Company
            {
                Name = "AAA",
            };
            var serializeObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/companies", postBody);

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(company.Id, saveCompany.Id);
            Assert.Equal(company.Name, saveCompany.Name);

            var response2 = await httpClient.PostAsync("/api/companies", postBody);
            Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
        }

        [Fact]
        public async Task Should_return_NoContent_when_delete_all()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            var response = await httpClient.DeleteAsync("/api/companies");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Should_get_all_companyies_successfullyAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            var company1 = new Company
            {
                Name = "AAA",
            };
            var company2 = new Company
            {
                Name = "AAB",
            };
            var companies = new List<Company>() { company1, company2 };
            var serializeObject1 = JsonConvert.SerializeObject(company1);
            var serializeObject2 = JsonConvert.SerializeObject(company2);
            var postBody1 = new StringContent(serializeObject1, Encoding.UTF8, "application/json");
            var postBody2 = new StringContent(serializeObject2, Encoding.UTF8, "application/json");

            _ = await httpClient.PostAsync("/api/companies", postBody1);
            _ = await httpClient.PostAsync("/api/companies", postBody2);

            var response = await httpClient.GetAsync("/api/companies");

            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            var compareResult = from item in companies
                            from item2 in saveCompanies
                            where item.Name == item2.Name
                            select item;
            Assert.Equal(2, compareResult.Count());
        }
    }
}
