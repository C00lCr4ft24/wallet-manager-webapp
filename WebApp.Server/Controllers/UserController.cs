using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Namotion.Reflection;
using System.Security.Claims;
using WebApp.Dal.Entities;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Contexts;
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace WebApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
	IUserService userService,
	IWalletService walletService,
	IRequestContextProvider contextProvider
	) : ControllerBase
{
	private readonly IUserService userService = userService;
	private readonly IWalletService walletService = walletService;
	private readonly IRequestContextProvider contextProvider = contextProvider;

	[HttpPost("register-user")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> RegisterUserAsync([FromBody] RegisterUserDto request, CancellationToken cancellationToken = default)
	{
		var command = new CreateUserCommand { Email = request.Email, Password = request.Password };
		UserHeaderDto newUser = await userService.RegisterUserAsync(command, cancellationToken);
		return Ok(newUser);
	}

	[HttpGet("current-user")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> GetCurrentUserInfoAsync(CancellationToken cancellationToken = default)
	{
		var command = new GetUserCommand { InvokerId = contextProvider.InvokerId, Id = contextProvider.InvokerId };
		UserHeaderDto user = await userService.GetUserAsync(command, cancellationToken);
		return Ok(user);
	}

	[HttpPost("update-email")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> UpdateUserEmailAsync([FromBody] UpdateUserEmailDto request, CancellationToken cancellationToken = default)
	{
		var command = new UpdateUserEmailCommand { InvokerId = contextProvider.InvokerId, Email = request.Email };
		UserHeaderDto updatedUser = await userService.UpdateUserEmailAsync(command, cancellationToken);
		return Ok(updatedUser);
	}

	[HttpPost("update-username")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> UpdateUserNameAsync([FromBody] UpdateUserNameDto request, CancellationToken cancellationToken = default)
	{
		var command = new UpdateUserNameCommand { InvokerId = contextProvider.InvokerId, UserName = request.UserName };
		UserHeaderDto updatedUser = await userService.UpdateUserNameAsync(command, cancellationToken);
		return Ok(updatedUser);
	}
	[HttpPost("update-dob")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> UpdateUserDateOfBirthAsync([FromBody] UpdateUserDateOfBirthDto request, CancellationToken cancellationToken = default)
	{
		var command = new UpdateUserDateOfBirthCommand { InvokerId = contextProvider.InvokerId, DateOfBirth = request.DateOfBirth };
		UserHeaderDto updatedUser = await userService.UpdateUserDateOfBirthAsync(command, cancellationToken);
		return Ok(updatedUser);
	}
	[HttpPost("update-password")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> UpdateUserPasswordAsync([FromBody] UpdateUserPasswordDto request, CancellationToken cancellationToken = default)
	{
		var command = new UpdateUserPasswordCommand { InvokerId = contextProvider.InvokerId, OldPassword = request.OldPassword, NewPassword = request.NewPassword };
		UserHeaderDto updatedUser = await userService.UpdateUserPasswordAsync(command, cancellationToken);
		return Ok(updatedUser);
	}

	[HttpDelete]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UserHeaderDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<UserHeaderDto>> DeleteUserAsync(CancellationToken cancellationToken = default)
	{
		var command = new DeleteUserCommand { InvokerId = contextProvider.InvokerId, Id = contextProvider.InvokerId };
		UserHeaderDto deletedUser = await userService.DeleteUserAsync(command, cancellationToken);
		return Ok(deletedUser);
	}

	[HttpGet("{userId:int}/wallets")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(IEnumerable<WalletDataDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<IEnumerable<WalletDataDto>>> GetUserWalletsAsync([FromRoute] int userId, [FromQuery] bool loadUsers = false, [FromQuery] bool loadTransactions = false, CancellationToken cancellationToken = default)
	{
		var command = 
			new GetWalletsCommand 
			{ 
				InvokerId = contextProvider.InvokerId, 
				UserId = userId, 
				LoadUsers = loadUsers, 
				LoadTransactions = loadTransactions 
			};
		var wallets = await walletService.GetWalletsAsync(command, cancellationToken);
		return Ok(wallets);
	}
}