using CompanyApi.Dto;
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
namespace CompanyApiTest
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_new_company_successfullyAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            var company = new Company
            {
                Name = "AAA",
            };
            var serializeObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/companies", postBody);

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(company.Name, saveCompany.Name);

            var response2 = await httpClient.PostAsync("/api/companies", postBody);
            Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
        }

        [Fact]
        public async Task Should_return_NoContent_when_delete_all()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            var response = await httpClient.DeleteAsync("/api/companies");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Should_get_all_companyies_successfullyAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            var company1 = new Company
            {
                Name = "AAA",
            };
            var company2 = new Company
            {
                Name = "AAB",
            };
            var companies = new List<Company>() { company1, company2 };
            var serializeObject1 = JsonConvert.SerializeObject(company1);
            var serializeObject2 = JsonConvert.SerializeObject(company2);
            var postBody1 = new StringContent(serializeObject1, Encoding.UTF8, "application/json");
            var postBody2 = new StringContent(serializeObject2, Encoding.UTF8, "application/json");

            _ = await httpClient.PostAsync("/api/companies", postBody1);
            _ = await httpClient.PostAsync("/api/companies", postBody2);

            var response = await httpClient.GetAsync("/api/companies");

            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            var compareResult = from item in companies
                            from item2 in saveCompanies
                            where item.Name == item2.Name
                            select item;
            Assert.Equal(2, compareResult.Count());
        }

        [Fact]
        public async Task Should_get_company_when_get_given_id_successfullyAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            var company1 = new Company
            {
                Name = "AAA",
            };

            var serializeObject1 = JsonConvert.SerializeObject(company1);
            var postBody1 = new StringContent(serializeObject1, Encoding.UTF8, "application/json");

            var responsePost = await httpClient.PostAsync("/api/companies", postBody1);
            var responseBody = await responsePost.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(responseBody);

            var response = await httpClient.GetAsync($"/api/companies/{company.Id}");

            var responseBodyGet = await response.Content.ReadAsStringAsync();
            var saveCompany = JsonConvert.DeserializeObject<Company>(responseBodyGet);

            Assert.Equal(company.Id, saveCompany.Id);
            Assert.Equal(company.Name, saveCompany.Name);
        }

        [Fact]
        public async Task Should_get_page_when_get_companyies_successfullyAsync()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            var company1 = new Company
            {
                Name = "AAA",
            };
            var company2 = new Company
            {
                Name = "AAB",
            };
            var companies = new List<Company>() { company1, company2 };
            var serializeObject1 = JsonConvert.SerializeObject(company1);
            var serializeObject2 = JsonConvert.SerializeObject(company2);
            var postBody1 = new StringContent(serializeObject1, Encoding.UTF8, "application/json");
            var postBody2 = new StringContent(serializeObject2, Encoding.UTF8, "application/json");

            _ = await httpClient.PostAsync("/api/companies", postBody1);
            _ = await httpClient.PostAsync("/api/companies", postBody2);

            var response = await httpClient.GetAsync("/api/companies?pageSize=1&pageIndex=1");

            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(1, saveCompanies.Count());
        }

        [Fact]
        public async Task Should_get_updated_company_when_update_give_new_companyies_successfullyAsync()
        {
            // init
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");

            // given
            var company = new Company
            {
                Name = "AAA",
            };
            var serializeObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companies", postBody);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            saveCompany.Name = "BBB";
            var serializeObjectPatch = JsonConvert.SerializeObject(saveCompany);
            var patchBody = new StringContent(serializeObjectPatch, Encoding.UTF8, "application/json");
            // when
            var responsePatch = await httpClient.PatchAsync($"/api/companies/{saveCompany.Id}", patchBody);
            responsePatch.EnsureSuccessStatusCode();
            var responsePatchBody = await responsePatch.Content.ReadAsStringAsync();
            var patchedCompany = JsonConvert.DeserializeObject<Company>(responsePatchBody);

            // then
            var responseGet = await httpClient.GetAsync($"/api/companies/{saveCompany.Id}");
            responseGet.EnsureSuccessStatusCode();
            var responseGethBody = await responseGet.Content.ReadAsStringAsync();
            var responseGetCompany = JsonConvert.DeserializeObject<Company>(responsePatchBody);
            Assert.Equal(responseGetCompany.Name, "BBB");
        }

        [Fact]
        public async Task Should_create_employee_when_create_new_employee_successfullyAsync()
        {
            // init
            HttpClient httpClient = await CleanMemory();
            // given
            var company = new Company
            {
                Name = "AAA",
            };
            Company saveCompany = await CreateACompany(httpClient, company);
            var employee = new Employee
            {
                Name = "aaa",
                Salary = 122,
            };
            Employee employeeNew = await CreateEmployee(httpClient, saveCompany, employee);

            Assert.Equal(employeeNew.CompanyId, saveCompany.Id);
        }

        [Fact]
        public async Task Should_get_employee_when_search_given_company_successfullyAsync()
        {
            // init
            HttpClient httpClient = await CleanMemory();
            // given
            var company = new Company
            {
                Name = "AAA",
            };
            Company saveCompany = await CreateACompany(httpClient, company);
            var employee = new Employee
            {
                Name = "aaa",
                Salary = 122,
            };
            var employee2 = new Employee
            {
                Name = "bbb",
                Salary = 121,
            };
            Employee employeeNew = await CreateEmployee(httpClient, saveCompany, employee);
            Employee employeeNew2 = await CreateEmployee(httpClient, saveCompany, employee2);

            var responseGet = await httpClient.GetAsync($"/api/companies/{saveCompany.Id}/employees");

            responseGet.EnsureSuccessStatusCode();
            var responseGethBody = await responseGet.Content.ReadAsStringAsync();
            var responseGetEmployees = JsonConvert.DeserializeObject<List<Employee>>(responseGethBody);

            Assert.Equal(responseGetEmployees[0].CompanyId, responseGetEmployees[1].CompanyId);
        }

        private static async Task<Employee> CreateEmployee(HttpClient httpClient, Company saveCompany, Employee employee)
        {
            var serializeEmployee = JsonConvert.SerializeObject(employee);
            var postEmployeeBody = new StringContent(serializeEmployee, Encoding.UTF8, "application/json");
            var responseEmployee = await httpClient.PostAsync($"/api/companies/{saveCompany.Id}/employees", postEmployeeBody);
            responseEmployee.EnsureSuccessStatusCode();
            var responseBodyEmployee = await responseEmployee.Content.ReadAsStringAsync();
            var employeeNew = JsonConvert.DeserializeObject<Employee>(responseBodyEmployee);
            return employeeNew;
        }

        private static async Task<Company> CreateACompany(HttpClient httpClient, Company company)
        {
            var serializeObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companies", postBody);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            return saveCompany;
        }

        private static async Task<HttpClient> CleanMemory()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = await httpClient.DeleteAsync("/api/companies");
            _ = await httpClient.DeleteAsync("/api/employees");
            return httpClient;
        }
    }
}
