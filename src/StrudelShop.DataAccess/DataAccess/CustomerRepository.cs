using Dapper;
using StrudelShop.DataAccess.DataAccess.Interfaces;
using StrudelShop.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DataAccess
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly IDbConnection _dbConnection;

		public CustomerRepository(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

		public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
		{
			return await _dbConnection.QueryAsync<Customer>("GetAllCustomers", commandType: CommandType.StoredProcedure);
		}

		public async Task<Customer> GetCustomerByIdAsync(int customerId)
		{
			return await _dbConnection.QuerySingleOrDefaultAsync<Customer>("GetCustomerById", new { CustomerId = customerId }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> CreateCustomerAsync(Customer customer)
		{
			return await _dbConnection.ExecuteAsync("CreateCustomer", new { customer.FirstName, customer.LastName, customer.Email, customer.PhoneNumber, customer.Address }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> UpdateCustomerAsync(Customer customer)
		{
			return await _dbConnection.ExecuteAsync("UpdateCustomer", new { customer.CustomerId, customer.FirstName, customer.LastName, customer.Email, customer.PhoneNumber, customer.Address }, commandType: CommandType.StoredProcedure);
		}

		public async Task<int> DeleteCustomerAsync(int customerId)
		{
			return await _dbConnection.ExecuteAsync("DeleteCustomer", new { CustomerId = customerId }, commandType: CommandType.StoredProcedure);
		}
	}
}
