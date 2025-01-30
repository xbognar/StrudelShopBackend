using DataAccess.DTOs;
using DataAccess.Models;
using DataAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StrudelShop.DataAccess.DataAccess;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;

		public AuthenticationService(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<LoginResponseDTO> AuthenticateAsync(string username, string password)
		{
			var isAdmin = username == _configuration["ADMIN_USERNAME"]
					   && password == _configuration["ADMIN_PASSWORD"];

			User user;
			if (isAdmin)
			{
				// If the credentials match the admin in .env, treat them as an admin
				user = new User
				{
					Username = username,
					Role = "Admin"
				};
			}
			else
			{
				// Otherwise, look up the user in the DB
				user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

				// If user doesn't exist or password doesn't match, return null
				if (user == null || !VerifyPassword(password, user.PasswordHash))
					return null;
			}

			// Generate JWT
			var token = GenerateJwtToken(user);
			return new LoginResponseDTO
			{
				UserId = user.UserID,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Role = user.Role,
				Token = token
			};
		}

		public async Task<bool> RegisterUserAsync(User newUser)
		{
			// Here we store the "plain" password in newUser.PasswordHash
			// Then we call "HashPassword" which returns the same string for now.
			newUser.PasswordHash = HashPassword(newUser.PasswordHash);
			newUser.Role = "User";
			await _context.Users.AddAsync(newUser);
			return await _context.SaveChangesAsync() > 0;
		}

		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["JWT_KEY"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
					new Claim(ClaimTypes.Name, user.Username),
					new Claim(ClaimTypes.Role, user.Role)
				}),
				Expires = DateTime.UtcNow.AddMinutes(
					double.Parse(_configuration["JWT_TOKEN_EXPIRY_MINUTES"])
				),
				Issuer = _configuration["JWT_ISSUER"],
				Audience = _configuration["JWT_AUDIENCE"],
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature
				)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		private bool VerifyPassword(string password, string storedHash)
		{
			return password == storedHash;
		}

		private string HashPassword(string password)
		{
			return password;
		}
	}
}
