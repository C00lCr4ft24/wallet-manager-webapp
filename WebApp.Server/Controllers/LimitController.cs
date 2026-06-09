using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Contexts;
using WebApp.Service.Interfaces;

namespace WebApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LimitController(
	ILimitService limitService,
	IRequestContextProvider contextProvider
	) : ControllerBase
{
	private readonly ILimitService limitService = limitService;
	private readonly IRequestContextProvider contextProvider = contextProvider;

	[HttpGet] //###################################################
	public async Task<ActionResult<IEnumerable<LimitDto>>> GetAllLimitsAsync(
		[FromQuery] int? userId,
		CancellationToken cancellationToken = default
		)
	{
		var command = new GetAllLimitsCommand
		{
			InvokerId = contextProvider.InvokerId,
			UserId = userId
		};
		var result = await limitService.GetAllLimitsAsync(command, cancellationToken);
		return Ok(result);
	}

	[HttpPost] //###################################################
	public async Task<ActionResult<LimitDto>> CreateLimitAsync(
		[FromBody] CreateLimitDto dto, 
		CancellationToken cancellationToken = default
		)
	{
		var command 
			= new CreateLimitCommand
			{
				InvokerId = contextProvider.InvokerId,
				UserId = dto.UserId,
				MaxAmount = dto.MaxAmount,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate,
				IncludeIncome = dto.IncludeIncome,
				Description = dto.Description,
				IsActive = dto.IsActive
			};
		var result = await limitService.CreateLimitAsync(command, cancellationToken);
		return Ok(result);
	}
	[HttpGet("{limitId:int}")] //###################################
	public async Task<ActionResult<LimitDto>> GetLimitAsync(
		[FromRoute] int limitId,
		[FromQuery] bool loadFamily = false,
		[FromQuery] bool loadUser = false,
		CancellationToken cancellationToken = default
		)
	{
		var command = new GetLimitByIdCommand	
		{
			InvokerId = contextProvider.InvokerId,
			LimitId = limitId,
			LoadFamily = loadFamily,
			LoadUser = loadUser
		};
		var result = await limitService.GetLimitAsync(command, cancellationToken);
		return Ok(result);
	}
	[HttpPut("{limitId:int}")] //###################################
	public async Task<ActionResult<LimitDto>> UpdateLimitAsync(
		[FromRoute] int limitId,
		[FromBody] UpdateLimitDto dto,
		CancellationToken cancellationToken = default
		)
	{
		var command = new UpdateLimitCommand
		{
			InvokerId = contextProvider.InvokerId,
			LimitId = limitId,
			MaxAmount = dto.MaxAmount,
			StartDate = dto.StartDate,
			EndDate = dto.EndDate,
			IsActive = dto.IsActive,
			Description = dto.Description
		};
		var result = await limitService.UpdateLimitAsync(command, cancellationToken);
		return Ok(result);
	}
	[HttpDelete("{limitId:int}")] //################################
	public async Task<ActionResult<LimitDto>> DeleteLimitAsync(
		[FromRoute] int limitId,
		CancellationToken cancellationToken = default
		)
	{
		var command = new DeleteLimitCommand
		{
			InvokerId = contextProvider.InvokerId,
			LimitId = limitId
		};
		var result = await limitService.DeleteLimitAsync(command, cancellationToken);
		return Ok(result);
	}
}