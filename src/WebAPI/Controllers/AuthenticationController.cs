using DataAccess.DTOs;
using DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

		[HttpPost("login")]
		public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest)
		{
			var response = await _authService.AuthenticateAsync(loginRequest.Username, loginRequest.Password);
			if (response == null)
				return Unauthorized("Invalid credentials");
			return Ok(response);
		}
	}

}
