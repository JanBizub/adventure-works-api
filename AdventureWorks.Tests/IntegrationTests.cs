using System.Net;
using System.Net.Http.Json;
using DependencyInjectionApi.Data;
using DependencyInjectionApi.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AdventureWorks.Tests;

public class CustomersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CustomersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCustomers_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/v1/customers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_ForInvalidId()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/v1/customers/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EditCustomer_ReturnsNoContent_ForValidUpdate()
    {
        var client = _factory.CreateClient();

        var updatedCustomer = new DimCustomer
        {
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Gender = "M"
        };

        var response = await client.PutAsJsonAsync("api/v1/customers/1", updatedCustomer);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsNoContent_ForValidId()
    {
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync("api/v1/customers/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}