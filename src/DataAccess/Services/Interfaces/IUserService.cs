using DataAccess.DTOs;
using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.Services.Interfaces
{
	public interface IUserService
	{
		Task<User> GetUserByIdAsync(int userId);
		Task<IEnumerable<User>> GetAllUsersAsync();
		Task CreateUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task DeleteUserAsync(int userId);
	}

}
