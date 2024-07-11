using StrudelShop.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess.Interfaces
{
	public interface ICustomerRepository
	{

		Task<IEnumerable<Customer>> GetAllCustomersAsync();
		
		Task<Customer> GetCustomerByIdAsync(int customerId);
		
		Task<int> CreateCustomerAsync(Customer customer);
		
		Task<int> UpdateCustomerAsync(Customer customer);
		
		Task<int> DeleteCustomerAsync(int customerId);
	
	}

}
