using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.Models;
using StrudelShop.DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;

		public CustomerService(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}

		public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
		{
			return await _customerRepository.GetAllCustomersAsync();
		}

		public async Task<Customer> GetCustomerByIdAsync(int customerId)
		{
			return await _customerRepository.GetCustomerByIdAsync(customerId);
		}

		public async Task CreateCustomerAsync(Customer customer)
		{
			await _customerRepository.CreateCustomerAsync(customer);
		}

		public async Task UpdateCustomerAsync(Customer customer)
		{
			await _customerRepository.UpdateCustomerAsync(customer);
		}

		public async Task DeleteCustomerAsync(int customerId)
		{
			await _customerRepository.DeleteCustomerAsync(customerId);
		}
	}
}
