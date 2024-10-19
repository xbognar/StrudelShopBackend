using Microsoft.AspNetCore.Mvc;
using StrudelShop.Services.Interfaces;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrudelShop.DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StrudelShop.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class CustomersController : ControllerBase
	{
		private readonly ICustomerService _customerService;

		public CustomersController(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		// GET: api/Customer
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
		{
			var customers = await _customerService.GetAllCustomersAsync();
			return Ok(customers);
		}

		// GET: api/Customer/{id}
		[HttpGet("{customerId}")]
		public async Task<ActionResult<Customer>> GetCustomerById(int customerId)
		{
			var customer = await _customerService.GetCustomerByIdAsync(customerId);
			if (customer == null)
			{
				return NotFound();
			}
			return Ok(customer);
		}

		// POST: api/Customer
		[HttpPost]
		public async Task<ActionResult> CreateCustomer(Customer customer)
		{
			await _customerService.CreateCustomerAsync(customer);
			return CreatedAtAction(nameof(GetCustomerById), new { customerId = customer.CustomerId }, customer);
		}

		// PUT: api/Customer/{id}
		[HttpPut("{customerId}")]
		public async Task<ActionResult> UpdateCustomer(int customerId, Customer customer)
		{
			if (customerId != customer.CustomerId)
			{
				return BadRequest();
			}

			await _customerService.UpdateCustomerAsync(customer);
			return NoContent();
		}

		// DELETE: api/Customer
		[HttpDelete("{customerId}")]
		public async Task<ActionResult> DeleteCustomer(int customerId)
		{
			await _customerService.DeleteCustomerAsync(customerId);
			return NoContent();
		}
	}
}
