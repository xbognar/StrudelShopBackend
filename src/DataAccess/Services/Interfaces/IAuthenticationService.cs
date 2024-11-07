using DataAccess.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Interfaces
{
	public interface IAuthenticationService
	{
		Task<LoginResponseDTO> AuthenticateAsync(string username, string password);
	}
}
