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

        /*[Fact]
        public async void Should_get_all_company_list_successfully()
        {
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
        }*/
    }
}
