using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Dal;
using WebApp.Dal.Entities;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;

namespace WebApp.Service.Services;

public class FamilyService(
	AppDbContext db,
	IMapper mapper,
	ILogger<FamilyService> logger,
	IValidationService validationService,
	ICommonService commonService
	) : IFamilyService
{
	private readonly AppDbContext db = db;
	private readonly IMapper mapper = mapper;
	private readonly ILogger<FamilyService> logger = logger;
	private readonly IValidationService validationService = validationService;
	private readonly ICommonService commonService = commonService;

	public async Task<FamilyInviteDto> CreateInviteAsync(CreateFamilyInviteCommand command, CancellationToken cancellationToken = default)
	{
		//VALIDATE EMAIL
		validationService.ValidateEmail(command.UserEmail);

		var familyId = await commonService.GetFamilyIdOfUserAsync(command.InvokerId, cancellationToken: cancellationToken);
		var userId = await commonService.GetUserIdByEmailAsync(command.UserEmail, cancellationToken: cancellationToken);

		User invoker = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);
		User user = await commonService.GetUserEntityAsync(userId, loadFamily: true, cancellationToken: cancellationToken);

		Family invokerFamily = await commonService.GetFamilyEntityAsync(familyId, loadUsers: true, cancellationToken: cancellationToken);

		var isInvokerAnOwner = await db.FamilyUsers.AnyAsync(fu => fu.FamilyId == familyId && fu.UserId == invoker.Id && fu.IsOwner, cancellationToken);
		var failedMsg = "Invite to " + invokerFamily.Name;
		if(invoker.FamilyUser.FamilyId != invokerFamily.Id)
		{
			throw new ForbiddenException(Messages.FailedToCreate(failedMsg), $"You '{invoker.Email}' are not a member of this family!");
		}
		if(!isInvokerAnOwner)
		{
			throw new ForbiddenException(Messages.FailedToCreate(failedMsg), $"You '{invoker.Email}' are not an owner of this family!");
		}
		if(invoker.Id == user.Id)
		{
			throw new BadRequestException(Messages.FailedToCreate(failedMsg), $"You '{invoker.Email}' cannot add yourself to the family!");
		}
		if(invoker.FamilyUser.FamilyId == user.FamilyUser.FamilyId)
		{
			throw new BadRequestException(Messages.FailedToCreate(failedMsg), $"The user '{user.Email}' is already a member of this family!");
		}

		//CREATE INVITE
		var invite = new FamilyInvite
		{
			FamilyId = invokerFamily.Id,
			InviterId = invoker.Id,
			InviteeId = user.Id,
			IsAccepted = false,
			IsAnswered = false,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = invoker.Id
		};
		//CHECK FOR EXISTING INVITE
		var existingInvite = await db.FamilyInvites.AnyAsync(i => i.FamilyId == invite.FamilyId && i.InviterId == invite.InviterId && i.InviteeId == invite.InviteeId, cancellationToken);
		if(existingInvite)
		{
			throw new ConflictException(Messages.FailedToCreate(failedMsg), $"An invite for user '{user.Email}' and family '{invokerFamily.Name}' already exists!");
		}

		await db.FamilyInvites.AddAsync(invite, cancellationToken);

		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<FamilyInviteDto>(invite);
	}
	public async Task<IEnumerable<FamilyInviteDto>> GetSentInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<int> invites = await commonService.GetFamilyInviteIdsByInviterIdAsync(command.InvokerId, includeAnswered: command.IncludeAnswered, cancellationToken: cancellationToken);
		List<FamilyInvite> inviteEntities = [];
		foreach(var invite in invites)
		{
			var inviteEntity = await commonService.GetFamilyInviteEntityAsync(invite, loadFamily: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
			inviteEntities.Add(inviteEntity);
		}
		return mapper.Map<IEnumerable<FamilyInviteDto>>(inviteEntities);
	}
	public async Task<IEnumerable<FamilyInviteDto>> GetReceivedInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default)
	{
		IEnumerable<int> invites = await commonService.GetFamilyInviteIdsByInviteeIdAsync(command.InvokerId, includeAnswered: command.IncludeAnswered, cancellationToken: cancellationToken);
		List<FamilyInvite> inviteEntities = [];
		foreach(var invite in invites)
		{
			var inviteEntity = await commonService.GetFamilyInviteEntityAsync(invite, loadFamily: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
			inviteEntities.Add(inviteEntity);
		}
		return mapper.Map<IEnumerable<FamilyInviteDto>>(inviteEntities);
	}
	public async Task<FamilyUserDto> RespondToInviteAsync(RespondToInviteCommand command, CancellationToken cancellationToken = default)
	{
		var invite = await commonService.GetFamilyInviteEntityAsync(command.InviteId, loadFamily: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
		var invitee = await commonService.GetUserEntityAsync(invite.InviteeId, loadFamily: true, cancellationToken: cancellationToken);
		if(invite.InviteeId != command.InvokerId)
		{
			var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
			throw new ForbiddenException("Failed to respond to invite", $"You '{invoker.Email}' are not the invitee of this invite!");
		}
		if(invite.IsAccepted)
		{
			throw new ConflictException("Failed to respond to invite", "This invite has already been accepted!");
		}

		//ACCEPT OR DECLINE INVITE
		invite.IsAnswered = true;
		invite.IsAccepted = command.Accept;
		invite.UpdatedAt = DateTimeOffset.UtcNow;
		invite.UpdatedByUserId = command.InvokerId;
		//MOVE USER TO FAMILY
		if(command.Accept)
		{
			invitee.FamilyUser.FamilyId = invite.FamilyId;
			invitee.FamilyUser.UserId = invitee.Id;
			invitee.FamilyUser.IsOwner = false;
			invitee.FamilyUser.JoinedAt = DateTimeOffset.UtcNow;
			invitee.FamilyUser.UpdatedAt = DateTimeOffset.UtcNow;
			invitee.FamilyUser.UpdatedByUserId = command.InvokerId;
		}
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<FamilyUserDto>(invite.Invitee.FamilyUser);
	}
	public async Task<FamilyInviteDto> DeleteInviteAsync(DeleteInviteCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		var invite = await commonService.GetFamilyInviteEntityAsync(command.InviteId, loadFamily: true, loadInviter: true, loadInvitee: true, cancellationToken: cancellationToken);
		//AUTHORIZATION
		if(invite.InviterId != command.InvokerId)
		{
			throw new ForbiddenException(Messages.FailedToDelete(invite), $"You '{invoker.Email}' are not the inviter of this invite!");
		}
		//DELETE & SAVE
		db.FamilyInvites.Remove(invite);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<FamilyInviteDto>(invite);
	}

	public async Task<IEnumerable<FamilyUserDto>> GetFamilyUsersAsync(GetFamilyUsersCommand command, CancellationToken cancellationToken = default)
	{
		var familyId = await commonService.GetFamilyIdOfUserAsync(command.InvokerId, cancellationToken: cancellationToken);
		var familyUsers = await db.FamilyUsers.Where(fu => fu.FamilyId == familyId).ToListAsync(cancellationToken);
		List<FamilyUser> familyUserEntities = [];
		foreach(var familyUser in familyUsers)
		{
			var familyUserEntity = await commonService.GetFamilyUserEntityAsync(familyUser.Id, loadUser: true, cancellationToken: cancellationToken);
			familyUserEntities.Add(familyUserEntity);
		}
		return mapper.Map<IEnumerable<FamilyUserDto>>(familyUserEntities);
	}

	public async Task<FamilyUserDto> GetFamilyUserAsync(GetFamilyUserCommand command, CancellationToken cancellationToken = default)
	{
		var areInSameFamily = await commonService.AreUsersInTheSameFamilyAsync(command.InvokerId, command.FamilyUserId, cancellationToken: cancellationToken);
		var familyUser = await commonService.GetFamilyUserEntityAsync(command.FamilyUserId, loadUser: true, cancellationToken: cancellationToken);
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToAccess(familyUser), $"You are not in the same family as the requested user!");
		}
		return mapper.Map<FamilyUserDto>(familyUser);
	}

	public async Task<FamilyUserDto> GetCurrentFamilyUserAsync(GetCurrentFamilyUserCommand command, CancellationToken cancellationToken = default)
	{
		var familyUser = await commonService.GetFamilyUserEntityAsync(command.InvokerId, loadUser: true, cancellationToken: cancellationToken);
		return mapper.Map<FamilyUserDto>(familyUser);
	}

	public async Task<FamilyUserDto> UpdateFamilyUserAsync(UpdateFamilyUserCommand command, CancellationToken cancellationToken = default)
	{
		var userFamilyUser = await commonService.GetFamilyUserEntityAsync(command.FamilyUserId, loadUser: true, cancellationToken: cancellationToken);
		var invokerFamilyUser = await commonService.GetFamilyUserEntityAsync(command.InvokerId, loadUser: true, cancellationToken: cancellationToken);
		var areInSameFamily = invokerFamilyUser.FamilyId == userFamilyUser.FamilyId;
		if(!areInSameFamily)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(userFamilyUser), $"You are not in the same family as the requested user!");
		}
		if(!invokerFamilyUser.IsOwner)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(userFamilyUser), $"You are not an owner of this family!");
		}
		var numOfOwners = await db.FamilyUsers.CountAsync(fu => fu.FamilyId == invokerFamilyUser.FamilyId && fu.IsOwner, cancellationToken);
		if(numOfOwners == 1)
		{
			throw new BadRequestException(Messages.FailedToUpdate(userFamilyUser), $"There must be at least one owner in the family!");
		}
		userFamilyUser.IsOwner = command.IsOwner;
		userFamilyUser.UpdatedAt = DateTimeOffset.UtcNow;
		userFamilyUser.UpdatedByUserId = command.InvokerId;

		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<FamilyUserDto>(userFamilyUser);
	}
}