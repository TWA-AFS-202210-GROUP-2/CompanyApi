using System;
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
            await PostOneCompany(company, httpClient);
            await PostOneCompany(new Company("schlumberger"), httpClient);
            
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
            var exp = await PostOneCompany(company, httpClient);
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
            
            await PostOneCompany(new Company("1"), httpClient);
            await PostOneCompany(new Company("2"), httpClient);
            await PostOneCompany(new Company("3"), httpClient);
            await PostOneCompany(new Company("4"), httpClient);


            //when
            var res = await httpClient.GetAsync($"/api/companies?index=2&pagesize=2");

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<List<Company>>(readAsStringAsync);
            Assert.Equal(2, deserializeObject.Count);
            Assert.Equal("3", deserializeObject.First().Name);
            Assert.Equal("4", deserializeObject.Last().Name);
        }
        [Fact]
        public async Task Should_modify_company_name()
        {
            //given
            var httpClient = await HttpClientInit();
            var c = new Company("Schlumberger");
            var expobj = await PostOneCompanyAndGetReturnCompany(c, httpClient);
            c.Name = "SLB";
            //when
            var res = await httpClient.PutAsync($"/api/companies/{expobj._guid}", ConvertObjCompanyToRequestBody(c));

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<Company>(readAsStringAsync);
            Assert.Equal(c.Name, deserializeObject.Name);
        }
        [Fact]
        public async Task Should_add_a_employee_to_company()
        {
            //given
            var httpClient = await HttpClientInit();
            var company = await PostOneCompanyAndGetReturnCompany(new Company("SLB"), httpClient);

            var employee = new Employee("lwr", 1);
            //when
            var res = await httpClient.PostAsync($"/api/companies/{company._guid}/employees", ConvertObjEmployeeToRequestBody(employee));

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<Employee>(readAsStringAsync);
            Assert.Equal(employee.Name, deserializeObject.Name);
        }
        [Fact]
        public async Task Should_return_all_employees_of_a_company()
        {
            //given
            var httpClient = await HttpClientInit();
            var company = await PostOneCompanyAndGetReturnCompany(new Company("SLB"), httpClient);
            await PostOneEmployeeAndGetReturnEmployee(httpClient, company, new Employee("lwr", 1));
            await PostOneEmployeeAndGetReturnEmployee(httpClient, company, new Employee("lj", 1));
            await PostOneEmployeeAndGetReturnEmployee(httpClient, company, new Employee("wzy", 1));
            //when
            var res = await httpClient.GetAsync($"/api/companies/{company._guid}/employees");
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<List<Employee>>(readAsStringAsync);
            Assert.Equal(3, deserializeObject.Count);
        }

        private static async Task<Employee> PostOneEmployeeAndGetReturnEmployee(HttpClient httpClient, Company company, Employee employee)
        {
            var res = await httpClient.PostAsync($"/api/companies/{company._guid}/employees",
                ConvertObjEmployeeToRequestBody(employee));

            //then
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var readAsStringAsync = await res.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<Employee>(readAsStringAsync);
            return deserializeObject;
        }

        private static StringContent ConvertObjCompanyToRequestBody(Company c)
        {
            var companyString = JsonConvert.SerializeObject(c);
            var stringContent = new StringContent(companyString, Encoding.UTF8, "application/json");
            return stringContent;
        }
        private static StringContent ConvertObjEmployeeToRequestBody(Employee c)
        {
            var companyString = JsonConvert.SerializeObject(c);
            var stringContent = new StringContent(companyString, Encoding.UTF8, "application/json");
            return stringContent;
        }

        private static async Task<Company> PostOneCompanyAndGetReturnCompany(Company c, HttpClient httpClient)
        {
            var exp = await PostOneCompany(c, httpClient);
            var expstr = await exp.Content.ReadAsStringAsync();
            var expobj = JsonConvert.DeserializeObject<Company>(expstr);
            return expobj;
        }

        private static async Task<HttpResponseMessage> PostOneCompany(Company company, HttpClient httpClient)
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
