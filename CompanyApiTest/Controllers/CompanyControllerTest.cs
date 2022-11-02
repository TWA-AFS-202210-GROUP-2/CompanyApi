using CompanyApi.Model;
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
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PostAsync("/companies", postBody);
            //then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async void Should_no_add_company_when_company_has_already_exist()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);
            var company2 = new Company(name: "SLB");
            var companyJson2 = JsonConvert.SerializeObject(company2);
            var postBody2 = new StringContent(companyJson2, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PostAsync("/companies", postBody2);
            //then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_obtain_all_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);
            var company2 = new Company(name: "thoughtworks");
            var companyJson2 = JsonConvert.SerializeObject(company2);
            var postBody2 = new StringContent(companyJson2, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody2);

            //when
            var response = await httpClient.GetAsync("/companies");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal("SLB", createdCompany[0].Name);
            Assert.Equal("thoughtworks", createdCompany[1].Name);
        }

        [Fact]
        public async void Should_obtain_exist_company()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
        }

        [Fact]
        public async void Should_return_selection_page_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var company2 = new Company(name: "thoughtworks2");
            var companyJson2 = JsonConvert.SerializeObject(company2);
            var postBody2 = new StringContent(companyJson2, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody2);
            var company3 = new Company(name: "thoughtworks3");
            var companyJson3 = JsonConvert.SerializeObject(company3);
            var postBody3 = new StringContent(companyJson3, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody3);
            var company4 = new Company(name: "thoughtworks4");
            var companyJson4 = JsonConvert.SerializeObject(company4);
            var postBody4 = new StringContent(companyJson4, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody4);

            //when
            await httpClient.GetAsync("/companies?pageSize=2&pageIndex=2");

            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal("thoughtworks3", createdCompany[0].Name);
            Assert.Equal("thoughtworks4", createdCompany[1].Name);
        }
    }
}
