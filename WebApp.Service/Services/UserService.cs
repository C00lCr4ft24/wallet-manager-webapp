using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WebApp.Dal;
using WebApp.Dal.Entities;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;

namespace WebApp.Service.Services;

public class UserService(
	AppDbContext db,
	UserManager<User> manager,
	IMapper mapper,
	ILogger<UserService> logger,
	ICommonService commonService,
	IValidationService validationService
	) : IUserService
{
	private readonly AppDbContext db = db;
	private readonly UserManager<User> manager = manager;
	private readonly IMapper mapper = mapper;
	private readonly ILogger<UserService> logger = logger;
	private readonly ICommonService commonService = commonService;
	private readonly IValidationService validationService = validationService;

	public async Task<UserHeaderDto> RegisterUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
	{
		//VALIDATION
		validationService.ValidateEmail(command.Email);

		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

		var user = 
			new User 
			{
				UserName = command.Email,
				Name = command.Email.Split('@')[0],
				Email = command.Email,
				CreatedAt = DateTimeOffset.UtcNow,
			};

		var result = await manager.CreateAsync(user, command.Password);

		if(!result.Succeeded)
		{
			await transaction.RollbackAsync(cancellationToken);
			string reason = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException(Messages.FailedToCreate("User"), reason);
		}

		user.CreatedByUserId = user.Id;
		user.UpdatedAt = DateTimeOffset.UtcNow;
		user.UpdatedByUserId = user.Id;

		await commonService.CreateFamilyAsync(
			new CreateFamilyCommand
			{
				Name = $"{user.Name}'s family",
				InvokerId = user.Id
			},
			cancellationToken
		);

		await transaction.CommitAsync(cancellationToken);

		return mapper.Map<UserHeaderDto>(user);
	}
	public async Task<UserHeaderDto> GetUserAsync(GetUserCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION & NOT FOUND
		var invokerUser = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);
		var user = await commonService.GetUserEntityAsync(command.Id, loadFamily: true, cancellationToken: cancellationToken);

		var isAuthorized = invokerUser.FamilyUser.FamilyId == user.FamilyUser.FamilyId;
		if(!isAuthorized)
		{
			throw new ForbiddenException(Messages.FailedToAccess(user), $"You '{invokerUser.Email}' and {user.Email} must be in the same family!");
		}
		return mapper.Map<UserHeaderDto>(user);
	}

	public async Task<UserHeaderDto> UpdateUserEmailAsync(UpdateUserEmailCommand command, CancellationToken cancellationToken = default)
	{
		//VALIDATION
		validationService.ValidateEmail(command.Email);

		//DUPLICATE CHECK
		var existingUser = await manager.FindByEmailAsync(command.Email);
		if(existingUser != null && existingUser.Id != command.InvokerId)
		{
			throw new ConflictException(Messages.FailedToUpdate(existingUser), $"Email '{command.Email}' is already in use by another user!");
		}

		//AUTHORIZATION & NOT FOUND
		var user = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);

		//UPDATE
		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

		
		user.Email = command.Email;
		user.UserName = command.Email;
		user.UpdatedAt = DateTimeOffset.UtcNow;
		user.UpdatedByUserId = command.InvokerId;
		
		await manager.SetEmailAsync(user, command.Email);
		var result = await manager.UpdateAsync(user);

		if(!result.Succeeded)
		{
			await transaction.RollbackAsync(cancellationToken);
			string reason = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException(Messages.FailedToUpdate(user), reason);
		}
		await transaction.CommitAsync(cancellationToken);
		await db.SaveChangesAsync(cancellationToken);

		return mapper.Map<UserHeaderDto>(user);
	}
	public async Task<UserHeaderDto> UpdateUserNameAsync(UpdateUserNameCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION & NOT FOUND
		var user = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);

		//UPDATE
		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

		user.Name = command.UserName;
		user.UpdatedAt = DateTimeOffset.UtcNow;
		user.UpdatedByUserId = command.InvokerId;

		var result = await manager.UpdateAsync(user);

		if(!result.Succeeded)
		{
			await transaction.RollbackAsync(cancellationToken);
			var reason = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException(Messages.FailedToUpdate(user), reason);
		}

		user.FamilyUser.Family.Name = $"{user.Name}'s family";

		await transaction.CommitAsync(cancellationToken);
		await db.SaveChangesAsync(cancellationToken);

		return mapper.Map<UserHeaderDto>(user);
	}
	public async Task<UserHeaderDto> UpdateUserPasswordAsync(UpdateUserPasswordCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION & NOT FOUND
		var user = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);

		//UPDATE
		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

		var result = await manager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
		user.UpdatedAt = DateTimeOffset.UtcNow;
		user.UpdatedByUserId = command.InvokerId;

		if(!result.Succeeded)
		{
			await transaction.RollbackAsync(cancellationToken);
			var reason = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException(Messages.FailedToUpdate(user), reason);
		}
		await transaction.CommitAsync(cancellationToken);
		await db.SaveChangesAsync(cancellationToken);

		return mapper.Map<UserHeaderDto>(user);
	}
	public async Task<UserHeaderDto> UpdateUserDateOfBirthAsync(UpdateUserDateOfBirthCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION & NOT FOUND
		var user = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		//UPDATE
		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
		
		user.DateOfBirth = command.DateOfBirth;
		user.UpdatedAt = DateTimeOffset.UtcNow;
		user.UpdatedByUserId = command.InvokerId;

		var result = await manager.UpdateAsync(user);
		if(!result.Succeeded)
		{
			await transaction.RollbackAsync(cancellationToken);
			var reason = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException(Messages.FailedToUpdate(user), reason);
		}
		await transaction.CommitAsync(cancellationToken);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<UserHeaderDto>(user);
	}

	public async Task<UserHeaderDto> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken = default)
	{
		//AUTHORIZATION & NOT FOUND
		var user = await commonService.GetUserEntityAsync(command.InvokerId, cancellationToken: cancellationToken);
		//DELETE
		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
		var result = await manager.DeleteAsync(user);
		if(!result.Succeeded)
		{
			await transaction.RollbackAsync(cancellationToken);
			string reason = string.Join(", ", result.Errors.Select(e => e.Description));
			throw new BadRequestException(Messages.FailedToDelete(user), reason);
		}
		await transaction.CommitAsync(cancellationToken);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<UserHeaderDto>(user);
	}
}