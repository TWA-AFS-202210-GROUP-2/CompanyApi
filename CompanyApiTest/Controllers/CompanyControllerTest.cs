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
            var company = new Company(name: "SLB");
            var httpClient = GetHttpClient();

            // when
            var response = await PostCompany(company, httpClient);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Company createCompany = await DeserializeCompany(response);
            Assert.Equal("SLB", createCompany.Name);
            Assert.NotEmpty(createCompany.CompanyID);
        }

        [Fact]
        public async void Should_comflict()
        {
            // given
            var httpClient = GetHttpClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            await PostCompany(company, httpClient);

            // when
            var response = await PostCompany(company, httpClient);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_return_all_companies_successfully()
        {
            // given
            var httpClient = GetHttpClient();
            await httpClient.DeleteAsync("/companies");
            var companyOne = new Company(name: "SLB");
            var companyTwo = new Company(name: "TW");
            var createResponseOne = await PostCompany(companyOne, httpClient);
            var createResponseTwo = await PostCompany(companyTwo, httpClient);
            var createCompanyOne = await DeserializeCompany(createResponseOne);
            var createCompanyTwo = await DeserializeCompany(createResponseTwo);
            var campanies = new List<Company>();
            campanies.Add(createCompanyOne);
            campanies.Add(createCompanyTwo);

            // when
            var response = await httpClient.GetAsync("/companies");

            // then
            var response_ = await response.Content.ReadAsStringAsync();
            var getCompanies = JsonConvert.DeserializeObject<List<Company>>(response_);
            Assert.Equal(campanies, getCompanies);
        }

        [Fact]
        public async void Should_return_company_by_companyID_successfully()
        {
            // given
            var httpClient = GetHttpClient();
            await httpClient.DeleteAsync("/companies");
            var company_one = new Company(name: "SLB");
            var company_tow = new Company(name: "TW");
            await PostCompany(company_one, httpClient);
            var createResponse = await PostCompany(company_tow, httpClient);
            var createCompany = await DeserializeCompany(createResponse);

            // when
            var response = await httpClient.GetAsync($"/companies/{createCompany.CompanyID}");

            // then
            var getCompany = await DeserializeCompany(response);
            Assert.Equal(createCompany, getCompany);
        }

        [Fact]
        public async void Should_return_page_range_companies_successfully()
        {
            // given
            var httpClient = GetHttpClient();
            await httpClient.DeleteAsync("/companies");
            var company_one = new Company(name: "SLB");
            var company_two = new Company(name: "TW");
            var company_three = new Company(name: "Tencent");
            await PostCompany(company_one, httpClient);
            await PostCompany(company_two, httpClient);
            await PostCompany(company_three, httpClient);

            // when
            var response = await httpClient.GetAsync("/companies?pageSize=1&pageIndex=2");

            // then
            var response_ = await response.Content.ReadAsStringAsync();
            var getCompanies = JsonConvert.DeserializeObject<List<Company>>(response_);
            Assert.Equal(company_two.Name, getCompanies[0].Name);
        }

        [Fact]
        public async void Should_return_update_companies_successfully()
        {
            // given
            var httpClient = GetHttpClient();
            await httpClient.DeleteAsync("/companies");
            var company_one = new Company(name: "SLB");
            var createResponse = await PostCompany(company_one, httpClient);
            var createCompany = await DeserializeCompany(createResponse);
            createCompany.Name = "TW";
            var companyJson = JsonConvert.SerializeObject(createCompany);
            var potBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            // when
            var response = await httpClient.PutAsync($"/companies/{createCompany.CompanyID}", potBody);

            // then
            var response_ = await response.Content.ReadAsStringAsync();
            var updateCompanies = JsonConvert.DeserializeObject<List<Company>>(response_);
            Assert.Equal(createCompany, updateCompanies[0]);
        }

        [Fact]
        public async void Should_add_new_employee_successfully()
        {
            // given
            var employee = new Employee(name: "xiaoming", salary: 2000);
            var httpClient = GetHttpClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var createResponse = await PostCompany(company, httpClient);
            var createCompany = await DeserializeCompany(createResponse);

            // when
            var response = await PostEmployee(createCompany.CompanyID, employee, httpClient);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Employee createEmployee = await DeserializeEmployee(response);
            Assert.Equal("xiaoming", createEmployee.Name);
            Assert.NotEmpty(createEmployee.EmployeeID);
        }

        private static HttpClient GetHttpClient()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            return httpClient;
        }

        private static async Task<HttpResponseMessage> PostCompany(Company company, HttpClient httpClient)
        {
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            return response;
        }

        private static async Task<Company> DeserializeCompany(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var createCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            return createCompany;
        }

        private static async Task<HttpResponseMessage> PostEmployee(string companyID, Employee employee, HttpClient httpClient)
        {
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"/companies/{companyID}/employees", postBody);
            return response;
        }

        private static async Task<Employee> DeserializeEmployee(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var createEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
            return createEmployee;
        }
    }
}
