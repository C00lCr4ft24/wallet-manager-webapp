using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Dal;
using WebApp.Dto;
using WebApp.Service.Exceptions;
using WebApp.Service.Commands;
using WebApp.Service.Interfaces;
using WebApp.Dal.Entities;

namespace WebApp.Service.Services;

public class TransactionService(
	AppDbContext db,
	IMapper mapper,
	IValidationService validationService,
	ICommonService commonService
	) : ITransactionService
{
	private readonly AppDbContext db = db;
	private readonly IMapper mapper = mapper;
	private readonly ICommonService commonService = commonService;
	private readonly IValidationService validationService = validationService;

	public async Task<TransactionDataDto> CreateTransactionAsync(CreateTransactionCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var wallet = 
			await commonService
			.GetWalletEntityAsync(
				command.WalletId, 
				loadUsers: true, 
				cancellationToken: cancellationToken
			);
		var invoker = 
			await commonService
			.GetUserEntityAsync(
				command.InvokerId, 
				loadFamily: true, 
				cancellationToken: cancellationToken
			);

		//AUTHORIZATION
		var isInvokerInWallet = 
			await db.WalletUsers.AnyAsync(wu => wu.WalletId == command.WalletId && wu.UserId == invoker.Id, cancellationToken);
		if(!isInvokerInWallet)
		{
			throw new ForbiddenException(Messages.FailedToCreate("Transaction"), $"You '{invoker.Email}' are not a member of the wallet!");
		}
		//VALIDATION & CREATE
		var id = await commonService.CreateTransactionAsync(command, cancellationToken);
		var transaction = await commonService.GetTransactionEntityAsync(id, loadWallet: true, cancellationToken);
		return mapper.Map<TransactionDataDto>(transaction);
	}

	public async Task<TransactionDataDto> GetTransactionAsync(GetTransactionCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);
		var transaction = await commonService.GetTransactionEntityAsync(command.Id, loadWallet: true, cancellationToken);
		var isInvokerInWallet = await db.WalletUsers.AnyAsync(wu => wu.WalletId == transaction.WalletId && wu.UserId == invoker.Id, cancellationToken);
		if(!isInvokerInWallet)
		{
			throw new ForbiddenException(Messages.FailedToAccess(transaction), $"You '{invoker.Email}' are not a member of the wallet!");
		}
		return mapper.Map<TransactionDataDto>(transaction);
	}

	public async Task<TransactionDataDto> UpdateTransactionAsync(UpdateTransactionCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var transaction = await commonService.GetTransactionEntityAsync(command.Id, loadWallet: true, cancellationToken);
		var newWallet = command.WalletId != null
			? await commonService.GetWalletEntityAsync((int)command.WalletId, loadUsers: true, loadTransactions: true, cancellationToken)
			: transaction.Wallet;
		if(command.CategoryId != null) await commonService.GetCategoryEntityAsync((int)command.CategoryId, loadTransactions: false, cancellationToken);
		//AUTHORIZATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var isInvokerInWallet = await db.WalletUsers.AnyAsync(wu => wu.WalletId == newWallet.Id && wu.UserId == invoker.Id, cancellationToken);
		if(!isInvokerInWallet)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(transaction), $"You '{invoker.Email}' are not a member of the wallet!");
		}
		//UPDATE
		decimal delta = command.Amount != null
			? (decimal)(command.Amount - transaction.Amount)
			: 0;
		transaction.Name = command.Name ?? transaction.Name;
		transaction.Description = command.Description ?? transaction.Description;
		transaction.Amount = command.Amount ?? transaction.Amount;
		transaction.Date = command.Date ?? transaction.Date;
		transaction.WalletId = command.WalletId ?? transaction.WalletId;
		transaction.CategoryId = command.CategoryId ?? transaction.CategoryId;
		transaction.UpdatedAt = DateTimeOffset.UtcNow;
		transaction.UpdatedByUserId = command.InvokerId;

		//VALIDATION & SAVE
		validationService.ValidateTransaction(transaction);

		newWallet.Balance += delta;

		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<TransactionDataDto>(transaction);
	}
	public async Task<TransactionDataDto> DeleteTransactionAsync(DeleteTransactionCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var transaction = await commonService.GetTransactionEntityAsync(command.Id, loadWallet: true, cancellationToken);
		var wallet = await commonService.GetWalletEntityAsync(transaction.WalletId, loadUsers: true, loadTransactions: true, cancellationToken);
		//AUTHORIZATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var isInvokerInWallet = await db.WalletUsers.AnyAsync(wu => wu.WalletId == wallet.Id && wu.UserId == invoker.Id, cancellationToken);
		if(!isInvokerInWallet)
		{
			throw new ForbiddenException(Messages.FailedToDelete(transaction), $"You '{invoker.Email}' are not a member of the wallet!");
		}

		wallet.Balance -= transaction.Amount;
		db.Transactions.Remove(transaction);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<TransactionDataDto>(transaction);
	}
	public async Task<IEnumerable<TransactionDataDto>> GetAllTransactionsAsync(GetAllTransactionsCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);

		var isWalletIdsValid = command.WalletIds == null || !command.WalletIds.Any();
		var walletIds =
			isWalletIdsValid
			? await db.WalletUsers.Where(wu => wu.UserId == invoker.Id).Select(wu => wu.WalletId).ToListAsync(cancellationToken)
			: command.WalletIds;

		var isCategoryIdsValid = command.CategoryIds == null || !command.CategoryIds.Any();
		var categoryIds =
			isCategoryIdsValid 
			? await db.Categories.Where(c => c.FamilyId == invoker.FamilyUser.FamilyId || c.FamilyId == null).Select(c => c.Id).ToListAsync(cancellationToken)
			: command.CategoryIds;

		var query = db.Transactions.AsQueryable();
		if(command.StartDate != null)
		{
			query = query.Where(t => t.Date >= command.StartDate);
		}
		if(command.EndDate != null)
		{
			query = query.Where(t => t.Date <= command.EndDate);
		}

		if(isCategoryIdsValid)
		{
			query = query.Where(t => (t.CategoryId != null && categoryIds!.Contains((int)t.CategoryId)) || t.CategoryId == null);
		}
		else if(command.CategoryIds != null && command.CategoryIds.Any())
		{
			query = query.Where(t => t.CategoryId != null && categoryIds!.Contains((int)t.CategoryId));
		}

		if(isWalletIdsValid)
		{
			query = query.Where(t => walletIds!.Contains(t.WalletId));
		}

		var transactions = await query.Include(t => t.Wallet).Include(t => t.Category).Include(t => t.CreatedByUser).ToListAsync(cancellationToken);
		return mapper.Map<IEnumerable<TransactionDataDto>>(transactions);
	}
}