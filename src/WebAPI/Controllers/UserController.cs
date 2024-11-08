using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	[Authorize(Roles = "Admin")]
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
		{
			var users = await _userService.GetAllUsersAsync();
			return Ok(users);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUserById(int id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null)
				return NotFound();
			return Ok(user);
		}

		[HttpPost]
		public async Task<ActionResult> CreateUser(User user)
		{
			await _userService.CreateUserAsync(user);
			return CreatedAtAction(nameof(GetUserById), new { id = user.UserID }, user);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateUser(int id, User user)
		{
			if (id != user.UserID)
				return BadRequest();

			await _userService.UpdateUserAsync(user);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			await _userService.DeleteUserAsync(id);
			return NoContent();
		}
	}
}
