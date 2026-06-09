using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApp.Dal;
using WebApp.Dal.Entities;
using WebApp.Dto;
using WebApp.Service.Commands;
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;

namespace WebApp.Service.Services;

public class CategoryService(
	AppDbContext db,
	ICommonService commonService,
	IValidationService validationService,
	IAuthorizationService authorizationService,
	IMapper mapper, 
	ILogger<CategoryService> logger
	) : ICategoryService
{
	private readonly AppDbContext db = db;
	private readonly IMapper mapper = mapper;
	private readonly ILogger<CategoryService> logger = logger;
	private readonly IValidationService validationService = validationService;
	private readonly ICommonService commonService = commonService;
	private readonly IAuthorizationService authorizationService = authorizationService;
	public async Task<CategoryHeaderDto> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND & AUTHORIZATION
		var invoker = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);
		//CREATE
		var category = new Category
		{
			FamilyId = invoker.FamilyUser.FamilyId,
			Name = command.Name,
			Description = command.Description,
			Icon = command.Icon,
			Color = command.Color,
			IsDefault = false,
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedByUserId = command.InvokerId
		};
		//VALIDATION
		validationService.ValidateCategory(category);
		if(command.Color != null) { validationService.ValidateColor(command.Color); }
		//SAVE
		await db.Categories.AddAsync(category);
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<CategoryHeaderDto>(category);
	}

	public async Task<CategoryHeaderDto> GetCategoryAsync(GetCategoryCommand command, CancellationToken cancellationToken = default)
	{
		var category = await commonService.GetCategoryEntityAsync(command.CategoryId, cancellationToken: cancellationToken);
		await authorizationService.DoesUserHaveAccessToCategoryAsync(command.InvokerId, command.CategoryId, cancellationToken);
		return mapper.Map<CategoryHeaderDto>(category);
	}

	public async Task<IEnumerable<CategoryHeaderDto>> GetCategoriesAsync(GetCategoriesCommand command, CancellationToken cancellationToken = default)
	{
		var user = await commonService.GetUserEntityAsync(command.InvokerId, loadFamily: true, cancellationToken: cancellationToken);
		var categories = db.Categories.Where(c => c.FamilyId == user.FamilyUser.FamilyId || c.IsDefault).ToList();
		return mapper.Map<IEnumerable<CategoryHeaderDto>>(categories);
	}

	public async Task<CategoryHeaderDto> UpdateCategoryAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND & AUTHORIZATION
		var category = await commonService.GetCategoryEntityAsync(command.CategoryId, cancellationToken: cancellationToken);
		await authorizationService.DoesUserHaveAccessToCategoryAsync(command.InvokerId, command.CategoryId, cancellationToken);
		
		if(category.IsDefault)
		{
			throw new ForbiddenException(Messages.FailedToUpdate(category), "Default categories cannot be updated!");
		}

		//UPDATE
		category.Name = command.Name				?? category.Name;
		category.Description = command.Description	?? category.Description;
		category.Icon = command.Icon				?? category.Icon;
		category.Color = command.Color				?? category.Color;
		category.UpdatedAt = DateTimeOffset.UtcNow;
		category.UpdatedByUserId = command.InvokerId;
		//VALIDATION
		validationService.ValidateCategory(category);
		if(command.Color != null) { validationService.ValidateColor(command.Color); }
		//SAVE
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<CategoryHeaderDto>(category);
	}
	public async Task<CategoryHeaderDto> DeleteCategoryAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
	{
		//NOT FOUND & AUTHORIZATION
		var category = await commonService.GetCategoryEntityAsync(command.CategoryId, cancellationToken: cancellationToken);
		await authorizationService.DoesUserHaveAccessToCategoryAsync(command.InvokerId, command.CategoryId, cancellationToken);
		//DELETE
		db.Categories.Remove(category);
		//SAVE
		await db.SaveChangesAsync(cancellationToken);
		return mapper.Map<CategoryHeaderDto>(category);
	}
}