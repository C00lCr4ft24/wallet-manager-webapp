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

public class CommonService(
	AppDbContext db,
	ILogger<CommonService> logger,
	IValidationService validationService
	) : ICommonService
{
	private readonly AppDbContext db = db;
	private readonly ILogger<CommonService> logger = logger;
	private readonly IValidationService validationService = validationService;

	//##################################################### COMMON #####################################################
	public async Task<bool> IsDateInRangeAsync(DateTimeOffset date, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
	{
		var dateOnly = DateOnly.FromDateTime(date.UtcDateTime);
		var isInRange = dateOnly >= startDate && dateOnly <= endDate;
		return isInRange;
	}
	public bool IsDateInRange(DateTimeOffset date, DateOnly startDate, DateOnly endDate)
	{
		var dateOnly = DateOnly.FromDateTime(date.UtcDateTime);
		var isInRange = dateOnly >= startDate && dateOnly <= endDate;
		return isInRange;
	}
	//##################################################### WALLET #####################################################
	public async Task<int> CreateWalletAsync(CreateWalletCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHENTICATION
		var user = await GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);

		//CREATION
		var dbTransaction = await db.Database.BeginTransactionAsync(cancellationToken);
		//WALLET
		var wallet = new Wallet()
		{
			Name = command.Name,
			Balance = 0,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId,
			UpdatedAt = DateTimeOffset.UtcNow,
			UpdatedByUserId = command.InvokerId
		};
		validationService.ValidateWallet(wallet);
		await db.Wallets.AddAsync(wallet, cancellationToken);
		await db.SaveChangesAsync(cancellationToken);

		var walletUser = new WalletUser()
		{
			UserId = user.Id,
			WalletId = wallet.Id,
			IsOwner = true,
			JoinedAt = DateTimeOffset.UtcNow,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId,
			UpdatedAt = DateTimeOffset.UtcNow,
			UpdatedByUserId = command.InvokerId
		};
		await db.WalletUsers.AddAsync(walletUser, cancellationToken);
		await db.SaveChangesAsync(cancellationToken);

		//TRANSACTION
		if(command.Balance != 0)
		{
			await CreateTransactionAsync(
				new CreateTransactionCommand
				{
					InvokerId = command.InvokerId,
					Name = "Initial Balance",
					Description = null,
					Amount = command.Balance,
					Date = DateTimeOffset.UtcNow,
					IsImplicit = true,
					WalletId = wallet.Id,
					CategoryId = null
				},
				cancellationToken
			);
			await db.SaveChangesAsync(cancellationToken);
		}

		await dbTransaction.CommitAsync(cancellationToken);

		return wallet.Id;
	}
	public async Task<Wallet> GetWalletEntityAsync(int walletId, bool loadUsers = false, bool loadTransactions = false, CancellationToken cancellationToken = default)
	{
		var query = db.Wallets.AsQueryable();

		if(loadUsers && loadTransactions)
		{
			query = query
				.Include(w => w.Transactions)
					.ThenInclude(t => t.Category)
				.Include(w => w.WalletUsers)
					.ThenInclude(wu => wu.User);
		}
		else if(loadTransactions)
		{
			query = query
				.Include(w => w.Transactions)
					.ThenInclude(t => t.Category);
		}
		else if(loadUsers)
		{
			query = query
				.Include(w => w.WalletUsers)
					.ThenInclude(wu => wu.User);
		}

		Wallet wallet = await query.SingleOrNotFoundAsync(w => w.Id == walletId, "ID", walletId, cancellationToken);
		return wallet;
	}
	public async Task<IEnumerable<int>> GetWalletIdsOfUserAsync(int userId, CancellationToken cancellationToken = default)
	{
		User user = await GetUserEntityAsync(userId, cancellationToken: cancellationToken);
		List<int> walletIds = await db.WalletUsers.Where(wu => wu.UserId == user.Id).Select(wu => wu.WalletId).ToListAsync(cancellationToken);
		return walletIds;
	}
	public async Task<IEnumerable<int>> GetUserIdsOfWalletAsync(int walletId, CancellationToken cancellationToken = default)
	{
		Wallet wallet = await GetWalletEntityAsync(walletId, cancellationToken: cancellationToken);
		List<int> userIds = await db.WalletUsers.Where(wu => wu.WalletId == wallet.Id).Select(wu => wu.UserId).ToListAsync(cancellationToken);
		return userIds;
	}
	//##################################################### TRANSACTION #####################################################
	public async Task<int> CreateTransactionAsync(CreateTransactionCommand command, CancellationToken cancellationToken = default)
	{
		if(command.CategoryId != null) await GetCategoryEntityAsync((int)command.CategoryId, cancellationToken: cancellationToken);
		var transaction = new Transaction
		{
			Name = command.Name,
			Description = command.Description,
			Amount = command.Amount,
			Date = command.Date,
			IsImplicit = command.IsImplicit,
			WalletId = command.WalletId,
			CategoryId = command.CategoryId,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId,
			UpdatedAt = DateTimeOffset.UtcNow,
			UpdatedByUserId = command.InvokerId
		};
		validationService.ValidateTransaction(transaction);
		var wallet = await GetWalletEntityAsync(command.WalletId, cancellationToken: cancellationToken);
		wallet.Balance += transaction.Amount;
		validationService.ValidateWallet(wallet);

		var users = await GetUserIdsOfWalletAsync(wallet.Id, cancellationToken);
		var limits = await db.Limits.Where(l => users.Contains(l.UserId)).ToListAsync(cancellationToken);
		foreach(var l in limits)
		{
			var includeIncome = l.IncludeIncome;
			var inRange = await IsDateInRangeAsync(transaction.Date, l.StartDate, l.EndDate, cancellationToken);
			if(!inRange) continue;
			//ADD EXPENSE
			if(transaction.Amount < 0)
			{
				l.CurrentAmount += -transaction.Amount;
			}
			if(includeIncome && transaction.Amount > 0)
			{
				l.CurrentAmount += transaction.Amount;
			}
			validationService.ValidateLimit(l);
		}
		db.Transactions.Add(transaction);
		await db.SaveChangesAsync(cancellationToken);

		return transaction.Id;
	}
	public async Task<Transaction> GetTransactionEntityAsync(int transactionId, bool loadWallet = false, CancellationToken cancellationToken = default)
	{
		var query = db.Transactions.AsQueryable();
		query = query.Include(t => t.Category).Include(t => t.CreatedByUser);
		if(loadWallet)
		{
			query = query.Include(t => t.Wallet);
		}
		Transaction transaction = await query.SingleOrNotFoundAsync(t => t.Id == transactionId, "ID", transactionId, cancellationToken);
		return transaction;
	}

	//##################################################### CATEGORY #####################################################
	public async Task<Category> GetCategoryEntityAsync(int categoryId, bool loadTransactions = false, CancellationToken cancellationToken = default)
	{
		var query = db.Categories.AsQueryable();
		if(loadTransactions)
		{
			query = query.Include(c => c.Transactions);
		}
		Category category = await query.SingleOrNotFoundAsync(c => c.Id == categoryId, "ID", categoryId, cancellationToken);
		return category;
	}

	//###################################################### LIMIT #####################################################

	public async Task<Limit> GetLimitEntityAsync(int limitId, bool loadUser = false, CancellationToken cancellationToken = default)
	{
		var query = db.Limits.AsQueryable();
		if(loadUser)
		{
			query = query.Include(l => l.User);
		}
		Limit limit = await query.SingleOrNotFoundAsync(l => l.Id == limitId, "ID", limitId, cancellationToken);
		return limit;
	}
	public async Task<IEnumerable<Limit>> GetLimitsOfUserAsync(int userId, CancellationToken cancellationToken = default)
	{
		List<Limit> limits = await db.Limits.Where(l => l.UserId == userId).ToListAsync(cancellationToken);
		return limits;
	}
	//###################################################### FAMILY #####################################################
	public async Task<int> GetFamilyIdOfUserAsync(int userId, CancellationToken cancellationToken = default)
	{
		var familyUser = await db.FamilyUsers
			.Include(fu => fu.Family)
			.SingleOrNotFoundAsync(fu => fu.UserId == userId, "ID", userId, cancellationToken);
		return familyUser.FamilyId;
	}
	public async Task<Family> GetFamilyEntityAsync(int familyId, bool loadUsers = false, bool loadCategories = false, CancellationToken cancellationToken = default)
	{
		var query = db.Families.AsQueryable();
		if(loadUsers)
		{
			query = query
				.Include(f => f.FamilyUsers)
					.ThenInclude(fu => fu.User);
		}
		if(loadCategories)
		{
			query = query
				.Include(f => f.Categories);
		}
		Family family = await query.SingleOrNotFoundAsync(f => f.Id == familyId, "ID", familyId, cancellationToken);
		return family;
	}
	public async Task<int> CreateFamilyAsync(CreateFamilyCommand command, CancellationToken cancellationToken = default)
	{
		var user = await GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var numOfFamilies = await db.FamilyUsers.CountAsync(fu => fu.UserId == user.Id, cancellationToken);
		if(numOfFamilies > 0)
		{
			throw new ForbiddenException(
				exceptionMsg: Messages.FailedToCreate("Family"),
				reasonMsg: $"User with email '{user.Email}' is already member of a family!"
				);
		}

		var family = new Family
		{
			Name = command.Name,
			IsShared = false,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId,
			UpdatedAt = DateTimeOffset.UtcNow,
			UpdatedByUserId = command.InvokerId
		};
		db.Families.Add(family);
		await db.SaveChangesAsync(cancellationToken);

		var familyUser = new FamilyUser
		{
			UserId = user.Id,
			FamilyId = family.Id,
			IsOwner = true,
			JoinedAt = DateTimeOffset.UtcNow,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId,
			UpdatedAt = DateTimeOffset.UtcNow,
			UpdatedByUserId = command.InvokerId
		};
		db.FamilyUsers.Add(familyUser);
		await db.SaveChangesAsync(cancellationToken);
		user.FamilyUserId = familyUser.Id;
		await db.SaveChangesAsync(cancellationToken);
		return family.Id;
	}

	public async Task<bool> AreUsersInTheSameFamilyAsync(int userId1, int userId2, CancellationToken cancellationToken = default)
	{
		var familyUser1 = await db.FamilyUsers.SingleOrNotFoundAsync(fu => fu.UserId == userId1, "ID", userId1, cancellationToken);
		var familyUser2 = await db.FamilyUsers.SingleOrNotFoundAsync(fu => fu.UserId == userId2, "ID", userId2, cancellationToken);
		var areInSameFamily = familyUser1.FamilyId == familyUser2.FamilyId;
		return areInSameFamily;
	}

	public async Task<FamilyUser> GetFamilyUserEntityAsync(int familyUserId, bool loadUser = false, CancellationToken cancellationToken = default)
	{
		var query = db.FamilyUsers.AsQueryable();
		if(loadUser)
		{
			query = query
				.Include(fu => fu.User);
		}
		FamilyUser familyUser = await query.Include(fu => fu.Family).SingleOrNotFoundAsync(fu => fu.Id == familyUserId, "ID", familyUserId, cancellationToken);
		return familyUser;
	}
	//###################################################### USER #####################################################
	public async Task<int> GetUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		var user = await db.Users.SingleOrNotFoundAsync(u => u.Id == userId, "ID", userId, cancellationToken);
		return user.Id;
	}
	public async Task<int> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var user = await db.Users.SingleOrNotFoundAsync(u => u.Email == email, "Email", email, cancellationToken);
		return user.Id;
	}
	public async Task<User> GetUserEntityAsync(int userId, bool loadFamily = false, bool loadWallets = false, bool loadLimits = false, CancellationToken cancellationToken = default)
	{
		var query = db.Users.AsQueryable();
		if(loadFamily)
		{
			query = query
				.Include(u => u.FamilyUser)
					.ThenInclude(fu => fu.Family);
		}
		if(loadWallets)
		{
			query = query
				.Include(u => u.WalletUsers)
					.ThenInclude(wu => wu.Wallet);
		}
		if(loadLimits)
		{
			query = query
				.Include(u => u.Limits);
		}
		User user = await query.SingleOrNotFoundAsync(u => u.Id == userId, "ID", userId, cancellationToken);
		return user;
	}

	//###################################################### INVITE #####################################################
	//FAMILY
	public async Task<FamilyInvite> GetFamilyInviteEntityAsync(int inviteId, bool loadFamily = false, bool loadInviter = false, bool loadInvitee = false, CancellationToken cancellationToken = default)
	{
		var query = db.FamilyInvites.AsQueryable();
		if(loadFamily)
		{
			query = query
				.Include(i => i.Family);
		}
		if(loadInviter)
		{
			query = query
				.Include(i => i.Inviter);
		}
		if(loadInvitee)
		{
			query = query
				.Include(i => i.Invitee);
		}
		FamilyInvite invite = await query.SingleOrNotFoundAsync(i => i.Id == inviteId, "ID", inviteId, cancellationToken);
		return invite;
	}
	public async Task<IEnumerable<int>> GetFamilyInviteIdsByInviteeIdAsync(int inviteeId, bool includeAnswered = false, CancellationToken cancellationToken = default)
	{
		var query = db.FamilyInvites.Where(i => i.InviteeId == inviteeId);
		if(!includeAnswered)
		{
			query = query.Where(i => !i.IsAnswered);
		}
		List<int> inviteIds = await query.Select(i => i.Id).ToListAsync(cancellationToken);
		return inviteIds;
	}
	public async Task<IEnumerable<int>> GetFamilyInviteIdsByInviterIdAsync(int inviterId, bool includeAnswered = false, CancellationToken cancellationToken = default)
	{
		var query = db.FamilyInvites.Where(i => i.InviterId == inviterId);
		if(!includeAnswered)
		{
			query = query.Where(i => !i.IsAnswered);
		}
		List<int> inviteIds = await query.Select(i => i.Id).ToListAsync(cancellationToken);
		return inviteIds;
	}
	public async Task<IEnumerable<int>> GetFamilyInviteIdsByFamilyIdAsync(int familyId, CancellationToken cancellationToken = default)
	{
		List<int> inviteIds = await db.FamilyInvites.Where(i => i.FamilyId == familyId).Select(i => i.Id).ToListAsync(cancellationToken);
		return inviteIds;
	}

	//WALLET
	public async Task<WalletInvite> GetWalletInviteEntityAsync(int inviteId, bool loadWallet = false, bool loadInviter = false, bool loadInvitee = false, CancellationToken cancellationToken = default)
	{
		var query = db.WalletInvites.AsQueryable();
		if(loadWallet)
		{
			query = query
				.Include(i => i.Wallet);
		}
		if(loadInviter)
		{
			query = query
				.Include(i => i.Inviter);
		}
		if(loadInvitee)
		{
			query = query
				.Include(i => i.Invitee);
		}
		WalletInvite invite = await query.SingleOrNotFoundAsync(i => i.Id == inviteId, "ID", inviteId, cancellationToken);
		return invite;
	}
	public async Task<IEnumerable<int>> GetWalletInviteIdsByWalletIdAsync(int walletId, bool includeAnswered = false, CancellationToken cancellationToken = default)
	{
		var query = db.WalletInvites.Where(i => i.WalletId == walletId);
		if(!includeAnswered)
		{
			query = query.Where(i => !i.IsAnswered);
		}
		List<int> inviteIds = await query.Select(i => i.Id).ToListAsync(cancellationToken);
		return inviteIds;
	}
	public async Task<IEnumerable<int>> GetAllSentWalletInviteIdsAsync(int inviterId, bool includeAnswered = false, CancellationToken cancellationToken = default)
	{
		var query = db.WalletInvites.Where(i => i.InviterId == inviterId);
		if(!includeAnswered)
		{
			query = query.Where(i => !i.IsAnswered);
		}
		List<int> inviteIds = await query.Select(i => i.Id).ToListAsync(cancellationToken);
		return inviteIds;
	}
	public async Task<IEnumerable<int>> GetAllReceivedWalletInviteIdsAsync(int inviteeId, bool includeAnswered = false, CancellationToken cancellationToken = default)
	{
		var query = db.WalletInvites.Where(i => i.InviteeId == inviteeId);
		if(!includeAnswered)
		{
			query = query.Where(i => !i.IsAnswered);
		}
		List<int> inviteIds = await query.Select(i => i.Id).ToListAsync(cancellationToken);
		return inviteIds;
	}
}