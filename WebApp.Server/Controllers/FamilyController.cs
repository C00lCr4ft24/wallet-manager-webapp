using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Contexts;
using WebApp.Service.Interfaces;

namespace WebApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FamilyController(
	IFamilyService familyService,
	IRequestContextProvider contextProvider
	) : ControllerBase
{
	private readonly IFamilyService familyService = familyService;
	private readonly IRequestContextProvider contextProvider = contextProvider;

	[HttpGet("users")]
	public async Task<ActionResult<IEnumerable<FamilyUserDto>>> GetFamilyUsersAsync(CancellationToken cancellationToken = default)
	{
		var command = new GetFamilyUsersCommand { InvokerId = contextProvider.InvokerId };
		IEnumerable<FamilyUserDto> familyUsers = await familyService.GetFamilyUsersAsync(command, cancellationToken);
		return Ok(familyUsers);
	}

	[HttpGet("user")]
	public async Task<ActionResult<FamilyUserDto>> GetCurrentFamilyUserAsync(CancellationToken cancellationToken = default)
	{
		var command = new GetCurrentFamilyUserCommand { InvokerId = contextProvider.InvokerId };
		FamilyUserDto familyUser = await familyService.GetCurrentFamilyUserAsync(command, cancellationToken);
		return Ok(familyUser);
	}

	[HttpGet("user/{userId:int}")]
	public async Task<ActionResult<FamilyUserDto>> GetFamilyUserAsync([FromRoute] int userId, CancellationToken cancellationToken = default)
	{
		var command = new GetFamilyUserCommand { InvokerId = contextProvider.InvokerId, FamilyUserId = userId };
		FamilyUserDto familyUser = await familyService.GetFamilyUserAsync(command, cancellationToken);
		return Ok(familyUser);
	}

	[HttpPost("invite/send")]
	public async Task<ActionResult<FamilyInviteDto>> CreateInviteAsync(CreateFamilyInviteDto dto, CancellationToken cancellationToken = default)
	{
		var command =
			new CreateFamilyInviteCommand
			{
				InvokerId = contextProvider.InvokerId,
				UserEmail = dto.UserEmail,
			};
		FamilyInviteDto invite = await familyService.CreateInviteAsync(command, cancellationToken);
		return Ok(invite);
	}
	[HttpGet("invite/sent")]
	public async Task<ActionResult<IEnumerable<FamilyInviteDto>>> GetSentInvitesAsync(
		[FromQuery] bool includeAnswered = false,
		CancellationToken cancellationToken = default
		)
	{
		var command = 
			new GetInvitesCommand 
			{ 
				InvokerId = contextProvider.InvokerId, 
				IncludeAnswered = includeAnswered
			};
		var invites = await familyService.GetSentInvitesAsync(command, cancellationToken);
		return Ok(invites);
	}

	[HttpGet("invite/received")]
	public async Task<ActionResult<IEnumerable<FamilyInviteDto>>> GetReceivedInvitesAsync(
		[FromQuery] bool includeAnswered = false,
		CancellationToken cancellationToken = default
		)
	{
		var command = 
			new GetInvitesCommand 
			{ 
				InvokerId = contextProvider.InvokerId, 
				IncludeAnswered = includeAnswered 
			};
		IEnumerable<FamilyInviteDto> invites = await familyService.GetReceivedInvitesAsync(command, cancellationToken);
		return Ok(invites);
	}
	[HttpPost("invite/respond")]
	public async Task<ActionResult<FamilyUserDto>> RespondToInviteAsync(RespondToInviteDto dto, CancellationToken cancellationToken = default)
	{
		var command =
			new RespondToInviteCommand
			{
				InvokerId = contextProvider.InvokerId,
				InviteId = dto.Id,
				Accept = dto.Accept
			};
		FamilyUserDto familyUserData = await familyService.RespondToInviteAsync(command, cancellationToken);
		return Ok(familyUserData);
	}
	[HttpDelete("invite/delete/{inviteId:int}")]
	public async Task<ActionResult<FamilyInviteDto>> DeleteInviteAsync(
		[FromRoute] int inviteId,
		CancellationToken cancellationToken = default
		)
	{
		var command =
			new DeleteInviteCommand
			{
				InvokerId = contextProvider.InvokerId,
				InviteId = inviteId
			};
		FamilyInviteDto invite = await familyService.DeleteInviteAsync(command, cancellationToken);
		return Ok(invite);
	}

	[HttpPut("user/{userId:int}")]
	public async Task<ActionResult<FamilyUserDto>> UpdateFamilyUserAsync(
		[FromRoute] int userId,
		[FromQuery] bool isOwner,
		CancellationToken cancellationToken = default
		)
	{
		var command =
			new UpdateFamilyUserCommand
			{
				InvokerId = contextProvider.InvokerId,
				IsOwner = isOwner,
				FamilyUserId = userId
			};
		FamilyUserDto familyUser = await familyService.UpdateFamilyUserAsync(command, cancellationToken);
		return Ok(familyUser);
	}
}
