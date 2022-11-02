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
            _ = await PostNewCompany(httpClient, new CompanyDto { Name = "Schlumberger" });
            _ = await PostNewCompany(httpClient, new CompanyDto { Name = "ThoughtWorks" });
            _ = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });
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

        [Fact]
        public async void Should_return_new_employee_successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var assignedCompany = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });

            var employeeDto = new EmployeeDto { Name = "YaoMeng" };
            var serializationObject = JsonConvert.SerializeObject(employeeDto);
            var postBody = new StringContent(serializationObject, encoding: Encoding.UTF8, mediaType: "application/json");
            //when
            var response = await httpClient.PostAsync($"/api/companies/{assignedCompany.CompanyID}/employees", postBody);
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
            Assert.Equal("YaoMeng", createdEmployee.Name);
        }

        [Fact]
        public async void Should_return_employees_of_company_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            var assignedCompany = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });

            var postBodyOne = new EmployeeDto { Name = "YaoMeng" };
            var postBodyTwo = new EmployeeDto { Name = "MengYu" };
            var employeeOne = PostNewEmployee(assignedCompany.CompanyID, postBodyOne);
            var employeeTwo = PostNewEmployee(assignedCompany.CompanyID, postBodyTwo);
            //when
            var response = await httpClient.GetAsync($"/api/companies/{assignedCompany.CompanyID}/employees");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
            Assert.Equal("YaoMeng", employees[0].Name);
        }

        [Fact]
        public async void Should_return_updated_employee_of_company_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            var assignedCompany = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });

            var postBodyOne = new EmployeeDto { Name = "YaoMeng" };
            var postBodyTwo = new EmployeeDto { Name = "MengYu" };
            var employeeOne = await PostNewEmployee(assignedCompany.CompanyID, postBodyOne);
            var employeeTwo = await PostNewEmployee(assignedCompany.CompanyID, postBodyTwo);
            var updatedPostBody = SetupEmployeePostBody(new EmployeeDto { Name = "ZhangSan" });
            //when
            var response = await httpClient.PutAsync($"/api/companies/{assignedCompany.CompanyID}/{employeeOne.ID}", updatedPostBody);
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<Employee>(responseBody);
            Assert.Equal("ZhangSan", employees.Name);
        }

        [Fact]
        public async void Should_return_ok_delete_employee_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            var assignedCompany = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });

            var postBodyOne = new EmployeeDto { Name = "YaoMeng" };
            var postBodyTwo = new EmployeeDto { Name = "MengYu" };
            var employeeOne = await PostNewEmployee(assignedCompany.CompanyID, postBodyOne);
            var employeeTwo = await PostNewEmployee(assignedCompany.CompanyID, postBodyTwo);
            //when
            var response = await httpClient.DeleteAsync($"/api/companies/{assignedCompany.CompanyID}/{employeeOne.ID}");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Should_return_ok_delete_company_and_employees_successfully()
        {
            //given
            var httpClient = SetUpHttpClients();
            var assignedCompany = await PostNewCompany(httpClient, new CompanyDto { Name = "slb" });

            var postBodyOne = new EmployeeDto { Name = "YaoMeng" };
            var postBodyTwo = new EmployeeDto { Name = "MengYu" };
            var employeeOne = await PostNewEmployee(assignedCompany.CompanyID, postBodyOne);
            var employeeTwo = await PostNewEmployee(assignedCompany.CompanyID, postBodyTwo);
            //when
            var response = await httpClient.DeleteAsync($"/api/companies/{assignedCompany.CompanyID}");
            //then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public async Task<Company> PostNewCompany(HttpClient httpClient, CompanyDto company)
        {
            var postBody = SetupPostBody(company);
            var response = await httpClient.PostAsync("/api/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            return createdCompany;
        }

        public async Task<Employee> PostNewEmployee(string companyID, EmployeeDto employeeDto)
        {
            var httpClient = SetUpHttpClients();
            var postBody = SetupEmployeePostBody(employeeDto);
            var response = await httpClient.PostAsync($"/api/companies/{companyID}/employees", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);

            return createdEmployee;
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

        public StringContent SetupEmployeePostBody(EmployeeDto company)
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
