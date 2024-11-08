using DataAccess.DTOs;
using DataAccess.Models;
using System.Threading.Tasks;

namespace DataAccess.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<LoginResponseDTO> AuthenticateAsync(string username, string password);
		Task<bool> RegisterUserAsync(User newUser);
	}
}
