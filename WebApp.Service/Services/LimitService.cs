
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Dal;
using WebApp.Dal.Entities;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Exceptions;
using WebApp.Service.Extensions;
using WebApp.Service.Interfaces;

namespace WebApp.Service.Services;

public class LimitService(
	AppDbContext db,
	IMapper mapper,
	ILogger<LimitService> logger,
	IValidationService validationService,
	ICommonService commonService
	) : ILimitService
{
	private readonly AppDbContext db = db;
	private readonly IMapper mapper = mapper;
	private readonly ILogger<LimitService> logger = logger;
	private readonly IValidationService validationService = validationService;
	private readonly ICommonService commonService = commonService;

	public async Task<LimitDto> CreateLimitAsync(CreateLimitCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(command.UserId, cancellationToken: cancellationToken);
		//AUTHORIZATION
		var invokerIsOwner = await db.FamilyUsers.AnyAsync(fu => fu.UserId == invoker.Id && fu.IsOwner, cancellationToken: cancellationToken);
		var areInSameFamily = await commonService.AreUsersInTheSameFamilyAsync(invoker.Id, user.Id, cancellationToken: cancellationToken);
		var addingLimitToSelf = invoker.Id == user.Id;
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToCreate("Limit"), $"User '{user.Email}' is not in the same family as you '{invoker.Email}'!");
		}
		if(!invokerIsOwner && !addingLimitToSelf)
		{
			throw new ForbiddenException(Messages.FailedToCreate("Limit"), $"Only family owners can add limits to other users!");
		}
		//CREATE & VALIDATE
		var limit =
			new Limit
			{
				UserId = user.Id,
				MaxAmount = command.MaxAmount,
				EndDate = command.EndDate,
				StartDate = command.StartDate,
				Description = command.Description,
				IsActive = command.IsActive,
				IncludeIncome = command.IncludeIncome,
				CurrentAmount = 0,
				IsDeleted = false,
				CreatedAt = DateTime.UtcNow,
				CreatedByUserId = invoker.Id
			};

		var transactions =
			await db.Transactions
				.Include(t => t.Wallet)
					.ThenInclude(w => w.WalletUsers)
				.Where(t => t.Wallet.WalletUsers.Any(wu => wu.UserId == user.Id))
				.ToListAsync(cancellationToken);

		transactions = transactions.Where(t => !t.IsImplicit && commonService.IsDateInRange(t.Date, limit.StartDate, limit.EndDate)).ToList();

		foreach(var t in transactions)
		{
			if(t.Amount < 0)
			{
				limit.CurrentAmount += -t.Amount;
			}
			else if(t.Amount > 0 && limit.IncludeIncome)
			{
				limit.CurrentAmount += t.Amount;
			}
		}

		if(!limit.IsActive) { limit.CurrentAmount = 0; }

		validationService.ValidateLimit(limit);

		//SAVE
		await db.Limits.AddAsync(limit, cancellationToken);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<LimitDto>(limit);
	}
	public async Task<LimitDto> GetLimitAsync(GetLimitByIdCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var limit = await commonService.GetLimitEntityAsync(command.LimitId, loadUser: true, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(limit.UserId, cancellationToken: cancellationToken);
		//AUTHORIZATION
		var invokerIsOwner = await db.FamilyUsers.AnyAsync(fu => fu.UserId == invoker.Id && fu.IsOwner, cancellationToken: cancellationToken);
		var areInSameFamily = await commonService.AreUsersInTheSameFamilyAsync(invoker.Id, user.Id, cancellationToken: cancellationToken);
		var readingOwnLimit = invoker.Id == user.Id;
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToAccess(limit), $"You '{invoker.Email}' are not in the same family as the limit owner '{user.Email}'!");
		}
		if(!invokerIsOwner && !readingOwnLimit)
		{
			throw new ForbiddenException(Messages.FailedToAccess(limit), $"You '{invoker.Email}' are not the owner of this limit!");
		}
		limit = await commonService.GetLimitEntityAsync(command.LimitId, loadUser: command.LoadUser, cancellationToken: cancellationToken);
		return mapper.Map<LimitDto>(limit);
	}
	public async Task<IEnumerable<LimitDto>> GetAllLimitsAsync(GetAllLimitsCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		IEnumerable<User> users = [];
		IEnumerable<Limit> limits = [];
		if(command.UserId == null)
		{
			var familyId = await commonService.GetFamilyIdOfUserAsync(invoker.Id, cancellationToken: cancellationToken);
			users = await db.FamilyUsers.Include(fu => fu.User).Where(fu => fu.FamilyId == familyId).Select(fu => fu.User).ToListAsync(cancellationToken);
		}
		if(command.UserId != null)
		{
			users = [await commonService.GetUserEntityAsync(command.UserId.Value, cancellationToken: cancellationToken)];
		}
		//AUTHORIZATION
		var invokerIsOwner = await db.FamilyUsers.AnyAsync(fu => fu.UserId == invoker.Id && fu.IsOwner, cancellationToken: cancellationToken);
		foreach(var user in users)
		{
			var areInSameFamily = await commonService.AreUsersInTheSameFamilyAsync(invoker.Id, user.Id, cancellationToken: cancellationToken);
			var readingOwnLimit = invoker.Id == user.Id;

			if(!areInSameFamily) { continue; }
			if(!invokerIsOwner && !readingOwnLimit) { continue; }

			var limitsOfUser = await db.Limits.Where(l => l.UserId == user.Id).ToListAsync(cancellationToken);
			limits = limits.Concat(limitsOfUser);
		}

		return mapper.Map<IEnumerable<LimitDto>>(limits);
	}
	public async Task<LimitDto> UpdateLimitAsync(UpdateLimitCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var limit = await commonService.GetLimitEntityAsync(command.LimitId, loadUser: true, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(limit.UserId, cancellationToken: cancellationToken);
		//AUTHORIZATION
		var invokerIsOwner = await db.FamilyUsers.AnyAsync(fu => fu.UserId == invoker.Id && fu.IsOwner, cancellationToken: cancellationToken);
		var areInSameFamily = await commonService.AreUsersInTheSameFamilyAsync(invoker.Id, user.Id, cancellationToken: cancellationToken);
		var readingOwnLimit = invoker.Id == user.Id;
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(limit), $"You '{invoker.Email}' are not in the same family as the limit owner '{user.Email}'!");
		}
		if(!invokerIsOwner && !readingOwnLimit)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(limit), $"You '{invoker.Email}' are not the owner of this limit!");
		}
		//UPDATE & VALIDATE
		limit.MaxAmount = command.MaxAmount ?? limit.MaxAmount;
		limit.StartDate = command.StartDate ?? limit.StartDate;
		limit.EndDate = command.EndDate ?? limit.EndDate;
		limit.IncludeIncome = command.IncludeIncome ?? limit.IncludeIncome;
		limit.IsActive = command.IsActive ?? limit.IsActive;
		limit.Description = command.Description ?? limit.Description;
		limit.UpdatedAt = DateTime.UtcNow;
		limit.UpdatedByUserId = command.InvokerId;

		var transactions =
			await db.Transactions
				.Include(t => t.Wallet)
					.ThenInclude(w => w.WalletUsers)
				.Where(t => t.Wallet.WalletUsers.Any(wu => wu.UserId == user.Id))
				.ToListAsync(cancellationToken);

		transactions = transactions.Where(t => !t.IsImplicit && commonService.IsDateInRange(t.Date, limit.StartDate, limit.EndDate)).ToList();

		foreach(var t in transactions)
		{
			if(t.Amount < 0)
			{
				limit.CurrentAmount += -t.Amount;
			}
			else if(t.Amount > 0 && limit.IncludeIncome)
			{
				limit.CurrentAmount += t.Amount;
			}
		}

		if(!limit.IsActive) { limit.CurrentAmount = 0; }

		validationService.ValidateLimit(limit);
		return mapper.Map<LimitDto>(limit);
	}
	public async Task<LimitDto> DeleteLimitAsync(DeleteLimitCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var limit = await commonService.GetLimitEntityAsync(command.LimitId, loadUser: true, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(limit.UserId, cancellationToken: cancellationToken);
		//AUTHORIZATION
		var invokerIsOwner = await db.FamilyUsers.AnyAsync(fu => fu.UserId == invoker.Id && fu.IsOwner, cancellationToken: cancellationToken);
		var areInSameFamily = await commonService.AreUsersInTheSameFamilyAsync(invoker.Id, user.Id, cancellationToken: cancellationToken);
		var readingOwnLimit = invoker.Id == user.Id;
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToDelete(limit), $"You '{invoker.Email}' are not in the same family as the limit owner '{user.Email}'!");
		}
		if(!invokerIsOwner && !readingOwnLimit)
		{
			throw new ForbiddenException(Messages.FailedToDelete(limit), $"You '{invoker.Email}' are not the owner of this limit!");
		}
		//DELETE & SAVE
		db.Limits.Remove(limit);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<LimitDto>(limit);
	}
}