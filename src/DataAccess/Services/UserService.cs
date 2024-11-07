using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using StrudelShop.DataAccess.DataAccess;
using StrudelShop.DataAccess.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;

		public UserService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<User> GetUserByIdAsync(int userId)
		{
			return await _context.Users.FindAsync(userId);
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task CreateUserAsync(User user)
		{
			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateUserAsync(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteUserAsync(int userId)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}
		}
	}

}
