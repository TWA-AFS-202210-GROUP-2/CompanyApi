using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_new_company_successfully()
        {
            /*
             * 1. create Application
             * 2. Create httpClient
             * 3. Prepare request body
             * 4. call api
             * 5 .verify status code
             * 6. verify response body
             */

            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            company._guid = null;
            var companyString = JsonConvert.SerializeObject(company);
            var stringContent = new StringContent(companyString, Encoding.UTF8, "application/json");
            await httpClient.DeleteAsync("/api/companies");
            //when
            var res = await httpClient.PostAsync("/api/companies", stringContent);

            //then
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
            Assert.Equal(company.Name, deserializeObject.Name);
        }
        [Fact]
        public async Task Should_add_new_company_unsuccessful()
        {
            //given
            var httpClient = await HttpClientInit();
            var company = new Company(name: "SLB");
            company._guid = null;
            var companyString = JsonConvert.SerializeObject(company);
            var stringContent = new StringContent(companyString, Encoding.UTF8, "application/json");
            
            await httpClient.PostAsync("/api/companies", stringContent);
            //when
            var res = await httpClient.PostAsync("/api/companies", stringContent);

            //then
            Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
        }
        [Fact]
        public async Task Should_return_all_companies()
        {
            //given
            var httpClient = await HttpClientInit();
            var company = new Company(name: "SLB");
            await PostOne(company, httpClient);
            await PostOne(new Company("schlumberger"), httpClient);
            
            //when
            var res = await httpClient.GetAsync("/api/companies");

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<List<Company>>(readAsStringAsync);
            Assert.Equal(2, deserializeObject.Count);
        }
        [Fact]
        public async Task Should_return_selected_company()
        {
            //given
            var httpClient = await HttpClientInit();
            var company = new Company("SLB");
            var exp = await PostOne(company, httpClient);
            var expstr = await exp.Content.ReadAsStringAsync();
            var expobj = JsonConvert.DeserializeObject<Company>(expstr);
            //when
            var res = await httpClient.GetAsync($"/api/companies/{expobj._guid}");

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
            Assert.Equal(company.Name, deserializeObject.Name);
        }
        [Fact]
        public async Task Should_return_selected_by_index_and_pagesize_amount_company()
        {
            //given
            var httpClient = await HttpClientInit();
            var company = new Company("SLB");
            var exp = await PostOne(company, httpClient);
            var expstr = await exp.Content.ReadAsStringAsync();
            var expobj = JsonConvert.DeserializeObject<Company>(expstr);
            //when
            var res = await httpClient.GetAsync($"/api/companies/{expobj._guid}");

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
            Assert.Equal(company.Name, deserializeObject.Name);
        }

        private static async Task<HttpResponseMessage> PostOne(Company company, HttpClient httpClient)
        {
            var companyString = JsonConvert.SerializeObject(company);
            var stringContent = new StringContent(companyString, Encoding.UTF8, "application/json");
            var res = await httpClient.PostAsync("/api/companies", stringContent);
            return res;
        }

        private static async Task<HttpClient> HttpClientInit()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/api/companies");
            return httpClient;
        }
    }
}
