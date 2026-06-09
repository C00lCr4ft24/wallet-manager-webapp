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
public class TransactionController(
	ITransactionService transactionService,
	IRequestContextProvider contextProvider
	) : ControllerBase
{
	private readonly ITransactionService transactionService = transactionService;
	private readonly IRequestContextProvider contextProvider = contextProvider;

	[HttpPost]
	public async Task<ActionResult<IEnumerable<TransactionDataDto>>> GetAllTransactionsAsync(
		GetAllTransactionsDto request, 
		CancellationToken cancellationToken = default
		)
	{
		var command =
			new GetAllTransactionsCommand
			{
				InvokerId = contextProvider.InvokerId,
				WalletIds = request.WalletIds,
				CategoryIds = request.CategoryIds,
				StartDate = request.startDate,
				EndDate = request.endDate
			};
		var transactions = await transactionService.GetAllTransactionsAsync(command, cancellationToken);
		return Ok(transactions);
	}
	
	[HttpGet("{transactionId:int}")] // Gets a transaction by id
	public async Task<ActionResult<TransactionDataDto>> GetTransaction([FromRoute] int transactionId, CancellationToken cancellationToken = default)
	{
		var command = 
			new GetTransactionCommand 
			{
				InvokerId = contextProvider.InvokerId,
				Id = transactionId 
			};
		var transaction = await transactionService.GetTransactionAsync(command, cancellationToken);
		return Ok(transaction);
	}	

	[HttpPut("{transactionId:int}")] // Updates a transaction by id
	public async Task<ActionResult<TransactionDataDto>> UpdateTransactionAsync([FromRoute] int transactionId, [FromBody] UpdateTransactionDto request, CancellationToken cancellationToken = default)
	{
		var command =
			new UpdateTransactionCommand
			{
				InvokerId = contextProvider.InvokerId,
				Id = transactionId,
				WalletId = request.WalletId,
				Name = request.Name,
				Description = request.Description,
				Amount = request.Amount,
				Date = request.Date,
				CategoryId = request.CategoryId
			};
		TransactionDataDto transaction = await transactionService.UpdateTransactionAsync(command, cancellationToken);
		return Ok(transaction);
	}

	[HttpDelete("{transactionId:int}")]
	public async Task<ActionResult<TransactionDataDto>> DeleteTransactionAsync([FromRoute] int transactionId, CancellationToken cancellationToken = default)
	{
		var command =
			new DeleteTransactionCommand
			{
				InvokerId = contextProvider.InvokerId,
				Id = transactionId
			};
		TransactionDataDto transaction = await transactionService.DeleteTransactionAsync(command, cancellationToken);
		return Ok(transaction);
	}
}