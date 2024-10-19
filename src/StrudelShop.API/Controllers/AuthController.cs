using Microsoft.AspNetCore.Mvc;
using StrudelShop.DataAccess.Services.Interfaces;
using StrudelShop.DataAccess.Models;

namespace StrudelShop.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] User user)
		{
			var token = _authService.Authenticate(user.Username, user.Password);

			if (token == null)
				return Unauthorized();

			return Ok(new { Token = token });
		}
	}
}
