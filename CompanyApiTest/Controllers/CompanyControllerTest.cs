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
        public async void Should_()
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
    }
}
