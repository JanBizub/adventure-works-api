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

    [HttpGet("{id}")]
    public IActionResult GetCustomer(int id)
    {
        var customer = _dbContext.DimCustomers
            .Where(c => c.CustomerKey == id)
            .Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Gender
            })
            .FirstOrDefault();

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPut("{id}")]
    public IActionResult EditCustomer(int id, [FromBody] DimCustomer updatedCustomer)
    {
        if (updatedCustomer == null)
        {
            return BadRequest("Updated customer data is required.");
        }

        var customer = _dbContext.DimCustomers.FirstOrDefault(c => c.CustomerKey == id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.FirstName = updatedCustomer.FirstName;
        customer.LastName = updatedCustomer.LastName;
        customer.Gender = updatedCustomer.Gender;

        _dbContext.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(int id)
    {
        var customer = _dbContext.DimCustomers.FirstOrDefault(c => c.CustomerKey == id);

        if (customer == null)
        {
            return NotFound();
        }

        _dbContext.DimCustomers.Remove(customer);
        _dbContext.SaveChanges();

        return NoContent();
    }
}