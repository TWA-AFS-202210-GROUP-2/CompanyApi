using CompanyApi.Dtos;
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
            var company = new CompanyDto { Name = "slb" };
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
            var company = new CompanyDto { Name = "slb" };
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

            _ = PostNewCompany(httpClient, new CompanyDto { Name = "slb" });
            _ = PostNewCompany(httpClient, new CompanyDto { Name = "Schlumberger" });
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

            var createdCompany = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });
            _ = PostNewCompany(httpClient, new CompanyDto { Name = "Schlumberger" });
            _ = PostNewCompany(httpClient, new CompanyDto { Name = "ThoughtWorks" });

            //when
            var response = await httpClient.GetAsync($"/api/companies/{createdCompany.CompanyID}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(responseBody);
            //then
            Assert.Equal("slb", company.Name);
        }

        [Fact]
        public async void Sould_return_companies_from_range_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            _ = PostNewCompany(httpClient, new CompanyDto { Name = "Schlumberger" });
            _ = PostNewCompany(httpClient, new CompanyDto { Name = "ThoughtWorks" });
            //when
            var response = await httpClient.GetAsync($"/api/companies?pageSize=1&pageIndex=1");
            var responseBody = await response.Content.ReadAsStringAsync();
            var companies = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            //then
            Assert.Single(companies);
        }

        [Fact]
        public async void Should_return_update_company_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new CompanyDto { Name = "Schlumberger" };
            var sericalizationObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(sericalizationObject, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/api/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            //when
            var updateCompany = new CompanyDto { Name = "slb" };
            var updateSericalizationObject = JsonConvert.SerializeObject(updateCompany);
            var updatedPostBody = new StringContent(updateSericalizationObject, encoding: Encoding.UTF8, mediaType: "application/json");
            var updatedResponse = await httpClient.PutAsync($"/api/companies/{createdCompany.CompanyID}", updatedPostBody);
            //then
            Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
            var updatedResponseBody = await updatedResponse.Content.ReadAsStringAsync();
            var updatedCompany = JsonConvert.DeserializeObject<Company>(updatedResponseBody);
            Assert.Equal("slb", updatedCompany.Name);
        }

        public async Task<Company> PostNewCompany(HttpClient httpClient, CompanyDto company)
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

        public StringContent SetupPostBody(CompanyDto company)
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
