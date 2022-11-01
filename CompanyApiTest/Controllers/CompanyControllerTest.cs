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
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            Assert.Equal(company.Name, createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public void Should_return_all_companies_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            //when
            //then
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
