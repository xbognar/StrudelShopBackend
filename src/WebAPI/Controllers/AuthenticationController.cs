using DataAccess.DTOs;
using DataAccess.Models;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationService _authService;

		public AuthenticationController(IAuthenticationService authService)
		{
			_authService = authService;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult> Register([FromBody] RegisterRequestDTO registerDto)
		{
			var newUser = new User
			{
				Username = registerDto.Username,
				PasswordHash = registerDto.Password
			};

			var isRegistered = await _authService.RegisterUserAsync(newUser);
			if (!isRegistered)
				return BadRequest("Registration failed.");

			return Ok("Registration successful");
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest)
		{
			var response = await _authService.AuthenticateAsync(
				loginRequest.Username,
				loginRequest.Password
			);

			if (response == null)
				return Unauthorized("Invalid credentials");

			return Ok(response);
		}
	}
}
