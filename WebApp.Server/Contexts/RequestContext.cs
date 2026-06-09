using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WebApp.Service.Exceptions;

namespace WebApp.Service.Contexts;

public interface IRequestContextProvider
{
	public int InvokerId { get; }
}

public class RequestContextProvider(IHttpContextAccessor httpContextAccessor) : IRequestContextProvider
{
	private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

	public int InvokerId
	{
		get
		{
			return int.TryParse(
				httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) 
				? userId : throw new UnauthorizedException("Failed to retrieve user information!", "User is not authenticated!");
		}
	}
}