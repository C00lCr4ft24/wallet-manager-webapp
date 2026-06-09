
using AutoMapper;
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
public class WalletController(
	IWalletService walletService, 
	ITransactionService transactionService,
	IRequestContextProvider contextProvider
	) : ControllerBase
{
	private readonly IWalletService walletService = walletService;
	private readonly ITransactionService transactionService = transactionService;
	private readonly IRequestContextProvider contextProvider = contextProvider;

	[HttpPost] // Creates a new wallet
	public async Task<ActionResult<WalletDataDto>> CreateWalletAsync(
		[FromBody] CreateWalletDto request,
		CancellationToken cancellationToken = default)
	{
		var command = 
			new CreateWalletCommand 
			{ 
				InvokerId = contextProvider.InvokerId, 
				Name = request.Name, 
				Balance = request.Balance,
			};
		var wallet = await walletService.CreateWalletAsync(command, cancellationToken);
		return Ok(wallet);
	}

	[HttpGet("{walletId:int}")] // Returns a specific wallet by ID
	public async Task<ActionResult<WalletDataDto>> GetWalletAsync([FromRoute] int walletId, [FromQuery] bool loadUsers = false, [FromQuery] bool loadTransactions = false, CancellationToken cancellationToken = default)
	{
		var command = 
			new GetWalletCommand 
			{
				InvokerId = contextProvider.InvokerId,
				Id = walletId,
				LoadUsers = loadUsers, 
				LoadTransactions = loadTransactions, 
			};
		var wallet = await walletService.GetWalletAsync(command, cancellationToken);
		return Ok(wallet);
	}

	[HttpGet] // Returns all wallets
	public async Task<ActionResult<IEnumerable<WalletDataDto>>> GetWalletsAsync([FromQuery] bool loadUsers = false, [FromQuery] bool loadTransactions = false, CancellationToken cancellationToken = default)
	{
		var command = 
			new GetWalletsCommand 
			{ 
				InvokerId = contextProvider.InvokerId,
				UserId = contextProvider.InvokerId,
				LoadTransactions = loadTransactions, 
				LoadUsers = loadUsers,
			};
		var wallets = await walletService.GetWalletsAsync(command, cancellationToken);
		var walletList = wallets.ToList();
		return Ok(walletList);
	}

	[HttpPut("{walletId:int}")] // Updates a specific wallet by ID
	public async Task<ActionResult<WalletDataDto>> UpdateWalletAsync([FromRoute] int walletId, [FromBody] UpdateWalletDto request, CancellationToken cancellationToken = default)
	{
		var command = 
			new UpdateWalletCommand 
			{ 
				InvokerId = contextProvider.InvokerId,
				Id = walletId, 
				Name = request.Name, 
				Balance = request.Balance,
			};
		var wallet = await walletService.UpdateWalletAsync(command, cancellationToken);
		return Ok(wallet);
	}

	[HttpDelete("{walletId:int}/user/{userId:int}")] // Removes User from Wallet
	public async Task<ActionResult<WalletDataDto>> RemoveUserFromWalletAsync([FromRoute] int walletId, [FromRoute] int userId, CancellationToken cancellationToken = default)
	{
		var command = 
			new RemoveUserFromWalletCommand
			{
				InvokerId = contextProvider.InvokerId,
				UserId = userId,
				WalletId = walletId,
			};
		var wallet = await walletService.RemoveUserFromWalletAsync(command, cancellationToken);
		return Ok(wallet);
	}

	[HttpDelete("{walletId:int}")] // Deletes a specific wallet by ID
	public async Task<ActionResult<WalletDataDto>> DeleteWallet([FromRoute] int walletId, CancellationToken cancellationToken = default)
	{
		var command = 
			new DeleteWalletCommand 
			{
				InvokerId = contextProvider.InvokerId,
				Id = walletId 
			};
		var wallet = await walletService.DeleteWalletAsync(command, cancellationToken);
		return Ok(wallet);
	}

	[HttpPut("{walletId:int}/user/{userId:int}")]
	public async Task<ActionResult<WalletDataDto>> ChangeUserRoleInWalletAsync([FromRoute] int walletId, [FromRoute] int userId, [FromQuery] bool isOwner, CancellationToken cancellationToken = default)
	{
		var command = 
			new ChangeUserRoleInWalletCommand 
			{
				InvokerId = contextProvider.InvokerId,
				WalletId = walletId,
				UserId = userId,
				IsOwner = isOwner
			};
		var wallet = await walletService.ChangeUserRoleInWalletAsync(command, cancellationToken);
		return Ok(wallet);
	}

	[HttpPost("{walletId:int}/invite/send")]
	public async Task<ActionResult<WalletInviteDto>> CreateInviteAsync([FromRoute] int walletId, [FromBody]	CreateWalletInviteDto request, CancellationToken cancellationToken = default)
	{
		var command =
			new CreateWalletInviteCommand
			{
				InvokerId = contextProvider.InvokerId,
				UserId = request.UserId,
				WalletId = walletId,
			};
		var invite = await walletService.CreateInviteAsync(command, cancellationToken);
		return Ok(invite);
	}
	[HttpGet("{walletId:int}/invite/sent")]
	public async Task<ActionResult<IEnumerable<WalletInviteDto>>> GetSentInvitesAsync(
		[FromRoute] int walletId,
		[FromQuery] bool includeAnswered = false,
		CancellationToken cancellationToken = default
		)
	{
		var command =
			new GetWalletInvitesCommand
			{
				InvokerId = contextProvider.InvokerId,
				WalletId = walletId,
				IncludeAnswered = includeAnswered,
			};
		var invites = await walletService.GetSentInvitesForWalletAsync(command, cancellationToken);
		return Ok(invites);
	}
	[HttpGet("invite/sent")]
	public async Task<ActionResult<IEnumerable<WalletInviteDto>>> GetAllSentInvitesAsync(
		[FromQuery] bool includeAnswered = false,
		CancellationToken cancellationToken = default
		)
	{
		var command =
			new GetInvitesCommand
			{
				InvokerId = contextProvider.InvokerId,
				IncludeAnswered = includeAnswered,
			};
		var invites = await walletService.GetAllSentWalletInvitesAsync(command, cancellationToken);
		return Ok(invites);
	}

	[HttpGet("{walletId:int}/invite/received")]
	public async Task<ActionResult<IEnumerable<WalletInviteDto>>> GetReceivedInvitesAsync(
		[FromRoute] int walletId,
		[FromQuery] bool includeAnswered = false,
		CancellationToken cancellationToken = default
		)
	{
		var command =
			new GetWalletInvitesCommand
			{
				InvokerId = contextProvider.InvokerId,
				WalletId = walletId,
				IncludeAnswered = includeAnswered,
			};
		var invites = await walletService.GetReceivedInvitesForWalletAsync(command, cancellationToken);
		return Ok(invites);
	}
	[HttpGet("invite/received")]
	public async Task<ActionResult<IEnumerable<WalletInviteDto>>> GetAllReceivedWalletInvitesAsync(
	[FromQuery] bool includeAnswered = false,
	CancellationToken cancellationToken = default
	)
	{
		var command =
			new GetInvitesCommand
			{
				InvokerId = contextProvider.InvokerId,
				IncludeAnswered = includeAnswered,
			};
		var invites = await walletService.GetAllReceivedWalletInvitesAsync(command, cancellationToken);
		return Ok(invites);
	}
	[HttpPost("invite/respond")]
	public async Task<ActionResult<WalletInviteDto>> RespondToInviteAsync([FromBody] RespondToInviteDto request, CancellationToken cancellationToken = default)
	{
		var command =
			new RespondToInviteCommand
			{
				InvokerId = contextProvider.InvokerId,
				InviteId = request.Id,
				Accept = request.Accept,
			};
		var invite = await walletService.RespondToInviteAsync(command, cancellationToken);
		return Ok(invite);
	}

	[HttpPost("{walletId:int}/transaction")]
	public async Task<IActionResult> CreateTransaction([FromRoute] int walletId, [FromBody] CreateTransactionDto request, CancellationToken cancellationToken = default)
	{
		var command = 
			new CreateTransactionCommand 
			{
				InvokerId = contextProvider.InvokerId,
				WalletId = walletId,
				CategoryId = request.CategoryId,
				Name = request.Name,
				Description = request.Description,
				Amount = request.Amount,
				Date = request.Date,
				IsImplicit = false
			};
		var transaction = await transactionService.CreateTransactionAsync(command, cancellationToken);
		return Ok(transaction);
	}

	[HttpGet("user/{userId:int}")]
	public async Task<ActionResult<WalletUserDto>> GetWalletUserAsync(
		[FromRoute] int userId,
		CancellationToken cancellationToken = default)
	{
		var command = new GetWalletUserCommand
		{
			InvokerId = contextProvider.InvokerId,
			UserId = userId
		};
		var walletUser = await walletService.GetWalletUserAsync(command, cancellationToken);
		return Ok(walletUser);
	}
}