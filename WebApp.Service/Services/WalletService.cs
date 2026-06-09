using Microsoft.EntityFrameworkCore;
using WebApp.Dal;
using WebApp.Dal.Entities;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using WebApp.Dto;
using WebApp.Service.Exceptions;
using AutoMapper;
using WebApp.Service.Commands;
using WebApp.Service.Interfaces;
using WebApp.Service.Extensions;

namespace WebApp.Service.Services;

public class WalletService(
	AppDbContext db,
	IMapper mapper,
	ILogger<WalletService> logger,
	IValidationService validationService,
	ICommonService commonService
	) : IWalletService
{
	private readonly AppDbContext db = db;
	private readonly IMapper mapper = mapper;
	private readonly ILogger<WalletService> logger = logger;
	private readonly ICommonService commonService = commonService;
	private readonly IValidationService validationService = validationService;

	public async Task<WalletUserDto> GetWalletUserAsync(GetWalletUserCommand command, CancellationToken cancellationToken = default)
	{
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		
		var walletUser = await db.WalletUsers.SingleOrNotFoundAsync(wu => wu.Id == command.UserId, "UserId", command.UserId, cancellationToken);
		var user = await commonService.GetUserEntityAsync(walletUser.UserId, loadFamily: true, loadWallets: true, cancellationToken: cancellationToken);
		
		var areInSameFamily = invoker.FamilyUser.FamilyId == user.FamilyUser.FamilyId;

		var wallet = await commonService.GetWalletEntityAsync(walletUser.WalletId, loadUsers: true, cancellationToken: cancellationToken);
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToAccess(wallet), $"You '{invoker.Email}' are not in the same family as {user.Email}!");
		}
		var areInSameWallet = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id) && wallet.WalletUsers.Any(wu => wu.UserId == user.Id);
		if(!areInSameWallet)
		{
			throw new ForbiddenException(Messages.FailedToAccess(wallet), $"You '{invoker.Email}' are not in the same wallet as {user.Email}!");
		}
		return mapper.Map<WalletUserDto>(walletUser);
	}

	public async Task<WalletHeaderDto> CreateWalletAsync(CreateWalletCommand command, CancellationToken cancellationToken = default)
	{
		var walletId = await commonService.CreateWalletAsync(command, cancellationToken);
		var wallet = await commonService.GetWalletEntityAsync(walletId, loadUsers: false, loadTransactions: false, cancellationToken: cancellationToken);
		return mapper.Map<WalletHeaderDto>(wallet);
	}

	public async Task<WalletHeaderDto> DeleteWalletAsync(DeleteWalletCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var wallet = await commonService.GetWalletEntityAsync(command.Id, loadUsers: true, loadTransactions: false, cancellationToken: cancellationToken);
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);

		//AUTHENTICATION
		var isUserInWallet = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id);
		var isUserAnOwner = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id && wu.IsOwner);
		if(!isUserInWallet)
		{
			throw new ForbiddenException(Messages.FailedToDelete(wallet), $"You '{invoker.Email}' are not a member of the wallet!");
		}
		if(!isUserAnOwner)
		{
			throw new ForbiddenException(Messages.FailedToDelete(wallet), $"You '{invoker.Email}' are not the owner of the wallet!");
		}
		db.Wallets.Remove(wallet);
		await db.SaveChangesAsync(cancellationToken);

		return mapper.Map<WalletHeaderDto>(wallet);
	}

	public async Task<IEnumerable<WalletDataDto>> GetWalletsAsync(GetWalletsCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHENTICATION
		var invokerId = await commonService.GetUserIdAsync(command.InvokerId, cancellationToken: cancellationToken);
		var invoker = await commonService.GetUserEntityAsync(invokerId, cancellationToken: cancellationToken);
		var invokerFamilyId = await commonService.GetFamilyIdOfUserAsync(invokerId, cancellationToken);
		var userId = await commonService.GetUserIdAsync(command.UserId, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(userId, cancellationToken: cancellationToken);
		var userFamilyId = await commonService.GetFamilyIdOfUserAsync(userId, cancellationToken);

		if(invokerFamilyId != userFamilyId)
		{
			throw new ForbiddenException("Failed to access wallets", $"You '{invoker.Email}' is not in the same family as {user.Email}!");
		}

		var walletIds = await commonService.GetWalletIdsOfUserAsync(command.UserId, cancellationToken);
		List<Wallet> wallets = [];
		foreach(var walletId in walletIds)
		{
			var wallet = await commonService.GetWalletEntityAsync(walletId, command.LoadUsers, command.LoadTransactions, cancellationToken: cancellationToken);
			wallets.Add(wallet);
		}

		return wallets.Select(w => mapper.Map<WalletDataDto>(w));
	}

	public async Task<WalletDataDto> GetWalletAsync(GetWalletCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var wallet = await commonService.GetWalletEntityAsync(command.Id, command.LoadUsers, command.LoadTransactions, cancellationToken: cancellationToken);

		var isInvokerInWallet = wallet.WalletUsers.Any(wu => wu.UserId == command.InvokerId);

		if(!isInvokerInWallet)
		{
			var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
			throw new ForbiddenException(Messages.FailedToAccess(wallet), $"You '{invoker.Email}' are not a member of the wallet!");
		}

		return mapper.Map<WalletDataDto>(wallet);
	}

	public async Task<WalletHeaderDto> UpdateWalletAsync(UpdateWalletCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var wallet = await commonService.GetWalletEntityAsync(command.Id, loadUsers: true, loadTransactions: false, cancellationToken);
		//AUTHENTICATION
		var user = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var isUserAnOwner = wallet.WalletUsers.Any(wu => wu.UserId == user.Id && wu.IsOwner);
		if(!isUserAnOwner)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"You '{user.Email}' are not an owner of this wallet!");
		}
		var delta = command.Balance - wallet.Balance;

		wallet.Name = command.Name;
		wallet.Balance = command.Balance;
		wallet.UpdatedAt = DateTimeOffset.UtcNow;
		wallet.UpdatedByUserId = command.InvokerId;

		if(delta != 0)
		{
			await commonService.CreateTransactionAsync(
				new CreateTransactionCommand()
				{
					InvokerId = command.InvokerId,
					Amount = delta,
					Name = "Balance Update",
					Description = null,
					Date = DateTimeOffset.UtcNow,
					IsImplicit = true,
					CategoryId = null,
					WalletId = wallet.Id,
				},
				cancellationToken
			);
		}

		validationService.ValidateWallet(wallet);

		db.Wallets.Update(wallet);
		await db.SaveChangesAsync(cancellationToken);

		return mapper.Map<WalletHeaderDto>(wallet);
	}

	public async Task<WalletDataDto> RemoveUserFromWalletAsync(RemoveUserFromWalletCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var wallet = await commonService.GetWalletEntityAsync(command.WalletId, loadUsers: true, loadTransactions: false, cancellationToken);
		var user = await commonService.GetUserEntityAsync(command.UserId, cancellationToken: cancellationToken);
		//AUTHENTICATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);

		var isInvokerInWallet = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id);
		var isInvokerAnOwner = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id && wu.IsOwner);

		var isUserInWallet = wallet.WalletUsers.Any(wu => wu.UserId == command.UserId);
		var isUserAnOwner = wallet.WalletUsers.Any(wu => wu.UserId == command.UserId && wu.IsOwner);

		var isRemovingSelf = invoker.Id == command.UserId;

		if(!isInvokerInWallet)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"You '{invoker.Email}' are not a member of the wallet!");
		}
		if(!isUserInWallet)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"User '{user.Email}' is not a member of the wallet!");
		}
		if(!isInvokerAnOwner && !isRemovingSelf)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"You '{invoker.Email}' are not an owner of this wallet!");
		}
		if(wallet.WalletUsers.Count == 1)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), "You cannot remove the only user from the wallet");
		}

		//REMOVAL
		wallet.WalletUsers.Remove(wallet.WalletUsers.First(wu => wu.UserId == command.UserId));
		wallet.UpdatedAt = DateTimeOffset.UtcNow;
		wallet.UpdatedByUserId = command.InvokerId;
		//VALIDATION & UPDATE
		validationService.ValidateWallet(wallet);
		db.Wallets.Update(wallet);
		await db.SaveChangesAsync(cancellationToken);

		return mapper.Map<WalletDataDto>(wallet);
	}

	public async Task<WalletInviteDto> CreateInviteAsync(CreateWalletInviteCommand command, CancellationToken cancellationToken = default)
	{

		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, loadWallets: true, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(command.UserId, loadFamily: true, loadWallets: true, cancellationToken: cancellationToken);
		var wallet = await commonService.GetWalletEntityAsync(command.WalletId, loadUsers: true, cancellationToken: cancellationToken);

		var isInvokerAnOwner = await db.WalletUsers.AnyAsync(wu => wu.WalletId == wallet.Id && wu.UserId == invoker.Id && wu.IsOwner, cancellationToken);
		var isUserInWallet = await db.WalletUsers.AnyAsync(wu => wu.WalletId == wallet.Id && wu.UserId == user.Id, cancellationToken);
		var areInSameFamily = invoker.FamilyUser.FamilyId == user.FamilyUser.FamilyId;
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToCreate($"Invite to {wallet.Name}"), $"You '{invoker.Email}' cannot invite user '{user.Email}' from a different family to the wallet!");
		}
		if(!isInvokerAnOwner)
		{
			throw new ForbiddenException(Messages.FailedToCreate($"Invite to {wallet.Name}"), $"You '{invoker.Email}' are not an owner of the wallet!");
		}
		if(invoker.Id == user.Id)
		{
			throw new BadRequestException(Messages.FailedToCreate($"Invite to {wallet.Name}"), $"You '{invoker.Email}' cannot invite yourself to the wallet!");
		}
		if(isUserInWallet)
		{
			throw new BadRequestException(Messages.FailedToCreate($"Invite to {wallet.Name}"), $"User '{user.Email}' is already a member of the wallet!");
		}
		//CREATE INVITE
		var invite = new WalletInvite
		{
			WalletId = wallet.Id,
			InviterId = invoker.Id,
			InviteeId = user.Id,
			IsAccepted = false,
			IsAnswered = false,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId,
		};
		//CHECK FOR EXISTING INVITE
		var existingInvite = await db.WalletInvites.AnyAsync(i => i.WalletId == invite.WalletId && i.InviterId == invite.InviterId && i.InviteeId == invite.InviteeId, cancellationToken);
		if(existingInvite)
		{
			throw new ConflictException(Messages.FailedToCreate($"Invite to {wallet.Name}"), $"An invite for user '{user.Email}' to the wallet already exists!");
		}
		await db.WalletInvites.AddAsync(invite, cancellationToken);

		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<WalletInviteDto>(invite);
	}
	public async Task<IEnumerable<WalletInviteDto>> GetSentInvitesForWalletAsync(GetWalletInvitesCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<int> invites = await commonService.GetWalletInviteIdsByWalletIdAsync(command.WalletId, includeAnswered: command.IncludeAnswered, cancellationToken: cancellationToken);
		List<WalletInvite> inviteEntities = [];
		foreach(var invite in invites)
		{
			var inviteEntity = await commonService.GetWalletInviteEntityAsync(invite, loadWallet: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
			if(inviteEntity.InviterId == command.InvokerId)
			{
				inviteEntities.Add(inviteEntity);
			}
		}
		return mapper.Map<IEnumerable<WalletInviteDto>>(inviteEntities);
	}
	public async Task<IEnumerable<WalletInviteDto>> GetAllSentWalletInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<int> invites = await commonService.GetAllSentWalletInviteIdsAsync(command.InvokerId, includeAnswered: command.IncludeAnswered, cancellationToken: cancellationToken);
		List<WalletInvite> inviteEntities = [];
		foreach(var invite in invites)
		{
			var inviteEntity = await commonService.GetWalletInviteEntityAsync(invite, loadWallet: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
			if(inviteEntity.InviterId == command.InvokerId)
			{
				inviteEntities.Add(inviteEntity);
			}
		}
		return mapper.Map<IEnumerable<WalletInviteDto>>(inviteEntities);
	}
	public async Task<IEnumerable<WalletInviteDto>> GetReceivedInvitesForWalletAsync(GetWalletInvitesCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<int> invites = await commonService.GetWalletInviteIdsByWalletIdAsync(command.WalletId, includeAnswered: command.IncludeAnswered, cancellationToken: cancellationToken);
		List<WalletInvite> inviteEntities = [];
		foreach(var invite in invites)
		{
			var inviteEntity = await commonService.GetWalletInviteEntityAsync(invite, loadWallet: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
			if(inviteEntity.InviteeId == command.InvokerId)
			{
				inviteEntities.Add(inviteEntity);
			}
		}
		return mapper.Map<IEnumerable<WalletInviteDto>>(inviteEntities);
	}
	public async Task<IEnumerable<WalletInviteDto>> GetAllReceivedWalletInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<int> invites = await commonService.GetAllReceivedWalletInviteIdsAsync(command.InvokerId, includeAnswered: command.IncludeAnswered, cancellationToken: cancellationToken);
		List<WalletInvite> inviteEntities = [];
		foreach(var invite in invites)
		{
			var inviteEntity = await commonService.GetWalletInviteEntityAsync(invite, loadWallet: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
			if(inviteEntity.InviteeId == command.InvokerId)
			{
				inviteEntities.Add(inviteEntity);
			}
		}
		return mapper.Map<IEnumerable<WalletInviteDto>>(inviteEntities);
	}
	public async Task<WalletUserDto> RespondToInviteAsync(RespondToInviteCommand command, CancellationToken cancellationToken = default)
	{
		var invite = await commonService.GetWalletInviteEntityAsync(command.InviteId, loadWallet: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
		var invitee = await commonService.GetUserEntityAsync(invite.InviteeId, loadWallets: true, cancellationToken: cancellationToken);
		if(invite.InviteeId != command.InvokerId)
		{
			var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
			throw new ForbiddenException("Failed to respond to invite", $"You '{invoker.Email}' are not the invitee of this invite!");
		}
		if(invite.IsAccepted)
		{
			throw new ConflictException("Failed to respond to invite", "This invite has already been accepted!");
		}
		db.WalletInvites.Remove(invite);

		var walletUser =
			new WalletUser()
			{
				UserId = invite.InviteeId,
				WalletId = invite.WalletId,
				IsOwner = false,
				JoinedAt = DateTimeOffset.UtcNow,
			};
		//MOVE USER TO FAMILY
		if(command.Accept)
		{
			invitee.WalletUsers.Add(walletUser);
		}
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<WalletUserDto>(walletUser);
	}

	public async Task<WalletDataDto> ChangeUserRoleInWalletAsync(ChangeUserRoleInWalletCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var wallet = await commonService.GetWalletEntityAsync(command.WalletId, loadUsers: true, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(command.UserId, cancellationToken: cancellationToken);
		//AUTHENTICATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);

		var isInvokerInWallet = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id);
		var isInvokerAnOwner = wallet.WalletUsers.Any(wu => wu.UserId == invoker.Id && wu.IsOwner);
		var isUserInWallet = wallet.WalletUsers.Any(wu => wu.UserId == command.UserId);
		var isUserAnOwner = wallet.WalletUsers.Any(wu => wu.UserId == command.UserId && wu.IsOwner);
		var isChangingSelf = invoker.Id == command.UserId;
		var ownersCount = wallet.WalletUsers.Count(wu => wu.IsOwner);

		if(!isInvokerInWallet)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"You '{invoker.Email}' are not a member of the wallet '{wallet.Name}'!");
		}
		if(!isUserInWallet)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"User '{user.Email}' is not a member of the wallet '{wallet.Name}'!");
		}
		if(!isInvokerAnOwner || !isInvokerAnOwner && !isChangingSelf)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"You '{invoker.Email}' are not the owner of the wallet '{wallet.Name}'!");
		}
		if(isUserAnOwner && ownersCount == 1 && !command.IsOwner)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(wallet), $"Cannot change ownership of the only owner from the wallet '{wallet.Name}'!");
		}
		var walletUser = await db.WalletUsers.SingleOrNotFoundAsync(wu => wu.UserId == command.UserId, "UserId", command.UserId, cancellationToken);

		walletUser.IsOwner = command.IsOwner;
		wallet.UpdatedAt = DateTimeOffset.UtcNow;
		wallet.UpdatedByUserId = command.InvokerId;
		db.Wallets.Update(wallet);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<WalletDataDto>(wallet);
	}
}