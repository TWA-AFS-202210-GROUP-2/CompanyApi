using CompanyApi.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var company = new Company
            {
                Name = "AAA",
                Id = Guid.NewGuid().ToString(),
            };
            var serializeObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializeObject, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/api/company", postBody);

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var saveCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(company.Id, saveCompany.Id);
            Assert.Equal(company.Name, saveCompany.Name);
        }
    }
}
