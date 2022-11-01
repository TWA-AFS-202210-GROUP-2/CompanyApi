using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            /*
                1.Create Application
                2.Create HttpClient
                3.Prepare request body(SerializeToJson,SerializeToHttpContent)
                4.call API
                5.verify status code
                6.verify response body(Deserialize)
             */
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createCompany.Name);
            Assert.NotEmpty(createCompany.CompanyID);
        }

        [Fact]
        public async void Should_comflict()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            // when
            await httpClient.PostAsync("/companies", postBody);
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_return_all_companies_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var postResponse = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await postResponse.Content.ReadAsStringAsync();
            var createCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            // when
            var response = await httpClient.GetAsync("/companies");

            // then
            var response_ = await response.Content.ReadAsStringAsync();
            var getCompanies = JsonConvert.DeserializeObject<List<Company>>(response_);
            Assert.Equal(createCompany, getCompanies[0]);
        }

        [Fact]
        public async void Should_return_page_range_companies_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company_one = new Company(name: "SLB");
            var company_two = new Company(name: "TW");
            var company_three = new Company(name: "Tencent");
            await PostCompany(company_one);
            await PostCompany(company_two);
            await PostCompany(company_three);

            // when
            var response = await httpClient.GetAsync("/companies?pageSize=1&pageIndex=2");

            // then
            var response_ = await response.Content.ReadAsStringAsync();
            var getCompanies = JsonConvert.DeserializeObject<List<Company>>(response_);
            Assert.Equal(company_two.Name, getCompanies[0].Name);
        }

        public async Task PostCompany(Company company)
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);
        }
    }
}
