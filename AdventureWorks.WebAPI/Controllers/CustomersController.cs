using DependencyInjectionApi.Data;
using DependencyInjectionApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AdventureWorksDw2022Context _dbContext;
    private readonly ICalculationService _calculationService;

    public CustomersController(AdventureWorksDw2022Context dbContext, ICalculationService calculationService)
    {
        _dbContext = dbContext;
        _calculationService = calculationService;
    }

    [HttpGet("echo")] public IActionResult Echo() => Ok("Echo");

    [HttpGet("calculate-discount")]
    public IActionResult CalculateDiscount(int originalPrice, int discountPercentage)
    {
        try
        {
            var discountedPrice = _calculationService.CalculateDiscount(originalPrice, discountPercentage);
            return Ok(new { OriginalPrice = originalPrice, DiscountPercentage = discountPercentage, DiscountedPrice = discountedPrice });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> GetCustomers(int pageNumber = 1, int customersPerPage = 10)
    {
        var maxRecordsRetrieved = 1000;

        if (pageNumber <= 0)
            throw new InvalidOperationException("PageNumber must be non-negative");
        if (customersPerPage > maxRecordsRetrieved)
            throw new InvalidOperationException($"There is a limit of max {maxRecordsRetrieved} customers per page.");

        var totalRecords = await _dbContext.DimCustomers.CountAsync();

        var skip = (pageNumber - 1) * customersPerPage;

        var customers = await _dbContext.DimCustomers
            .OrderBy(c => c.LastName)
            .Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Gender
            })
            .Skip(skip)
            .Take(customersPerPage)
            .ToListAsync();

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
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _dbContext.DimCustomers
            .Where(c => c.CustomerKey == id)
            .Select(c => new
            {
                c.FirstName,
                c.LastName,
                c.Gender
            })
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditCustomer(int id, [FromBody] DimCustomer? updatedCustomer)
    {
        if (updatedCustomer == null)
        {
            return BadRequest("Updated customer data is required.");
        }

        var customer = await _dbContext.DimCustomers.FirstOrDefaultAsync(c => c.CustomerKey == id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.FirstName = updatedCustomer.FirstName;
        customer.LastName = updatedCustomer.LastName;
        customer.Gender = updatedCustomer.Gender;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _dbContext.DimCustomers.FirstOrDefaultAsync(c => c.CustomerKey == id);

        if (customer == null)
        {
            return NotFound();
        }

        _dbContext.DimCustomers.Remove(customer);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}