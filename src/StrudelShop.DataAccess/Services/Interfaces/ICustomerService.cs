using StrudelShop.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.Services.Interfaces
{
	public interface ICustomerService
	{
		
		Task<IEnumerable<Customer>> GetAllCustomersAsync();
		
		Task<Customer> GetCustomerByIdAsync(int customerId);
		
		Task CreateCustomerAsync(Customer customer);
		
		Task UpdateCustomerAsync(Customer customer);
		
		Task DeleteCustomerAsync(int customerId);
	
	}
}
