using CompanyApi;
using Microsoft.AspNetCore.Http;
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

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "slb");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            // when
            var response = await httpClient.PostAsync("api/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            Assert.Equal(company.Name, createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async void Should_return_confict_message_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            var company = new Company(name: "slb");
            var postBody = SetupPostBody(company);

            // when
            var response = await httpClient.PostAsync("/api/companies", postBody);
            var responseTwo = await httpClient.PostAsync("/api/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Conflict, responseTwo.StatusCode);
        }

        [Fact]
        public async void Should_return_all_companies_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();

            PostNewCompany(httpClient, new Company(name: "slb"));
            PostNewCompany(httpClient, new Company(name: "Schlumberger"));
            // when
            var response = await httpClient.GetAsync("/api/companies");
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Should_return_an_existing_company_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();

            var createdCompany = await PostNewCompany(httpClient, new Company(name: "slb"));
            PostNewCompany(httpClient, new Company(name: "Schlumberger"));
            PostNewCompany(httpClient, new Company(name: "thoughtworks"));

            //when
            var response = await httpClient.GetAsync($"/api/companies/{createdCompany.CompanyID}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(responseBody);
            //then
            Assert.Equal("slb", company.Name);
        }

        public async Task<Company> PostNewCompany(HttpClient httpClient, Company company)
        {
            var postBody = SetupPostBody(company);
            var response = await httpClient.PostAsync("/api/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            return createdCompany;
        }

        public HttpClient SetUpHttpClients()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            return httpClient;
        }

        public StringContent SetupPostBody(Company company)
        {
            var serializeObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializeObject, Encoding.UTF8, mediaType: "application/json");

            return postBody;
        }

        public async Task<List<Company>> GetDeserializeObject(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            return companies;
        }
    }
}
