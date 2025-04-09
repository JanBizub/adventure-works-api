using DependencyInjectionApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjectionApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AdventureWorksDw2022Context _dbContext;

    public CustomersController(AdventureWorksDw2022Context dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("echo")] public IActionResult Echo() => Ok("Echo");

    [HttpGet("")]
    public IActionResult GetCustomers(int pageNumber = 1, int customersPerPage = 10)
    {
        var maxRecordsRetrieved = 1000;

        if (pageNumber <= 0)
            throw new InvalidOperationException("PageNumber must be non-negative");
        if (customersPerPage > maxRecordsRetrieved)
            throw new InvalidOperationException($"There is a limit of max {maxRecordsRetrieved} customers per page.");

        var totalRecords = _dbContext.DimCustomers.Count();

        var skip = (pageNumber - 1) * customersPerPage;

        var customers = _dbContext.DimCustomers
            .OrderBy(c => c.LastName)
            .Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Gender
            })
            .Skip(skip)
            .Take(customersPerPage)
            .ToList();

        var response = new
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            CustomersPerPage = customersPerPage,
            Customers = customers
        };

        return Ok(response);
    }
}