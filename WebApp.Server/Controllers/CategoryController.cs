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
public class CategoryController(
	ICategoryService categoryService,
	IRequestContextProvider requestContextProvider
	) : ControllerBase
{
	private readonly ICategoryService categoryService = categoryService;
	private readonly IRequestContextProvider requestContextProvider = requestContextProvider;


	[HttpPost]
	public async Task<ActionResult<CategoryHeaderDto>> CreateCategoryAsync([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken = default)
	{
		var command =
			new CreateCategoryCommand
			{
				InvokerId = requestContextProvider.InvokerId,
				Name = dto.Name,
				Description = dto.Description,
				Icon = dto.Icon,
				Color = dto.Color
			};
		var category = await categoryService.CreateCategoryAsync(command, cancellationToken);
		return Ok(category);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<CategoryHeaderDto>> GetCategoryByIdAsync([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var command = new GetCategoryCommand { InvokerId = requestContextProvider.InvokerId, CategoryId = id };
		var category = await categoryService.GetCategoryAsync(command, cancellationToken);
		return Ok(category);
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<CategoryHeaderDto>>> GetCategoriesAsync(CancellationToken cancellationToken = default)
	{
		var command = new GetCategoriesCommand { InvokerId = requestContextProvider.InvokerId };
		var categories = await categoryService.GetCategoriesAsync(command, cancellationToken);
		return Ok(categories);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<CategoryHeaderDto>> UpdateCategoryAsync([FromRoute] int id, [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken = default)
	{
		var command = new UpdateCategoryCommand
		{
			InvokerId = requestContextProvider.InvokerId,
			CategoryId = id,
			Name = dto.Name,
			Description = dto.Description,
			Icon = dto.Icon,
			Color = dto.Color
		};
		var category = await categoryService.UpdateCategoryAsync(command, cancellationToken);
		return Ok(category);
	}

	[HttpDelete("{id:int}")]
	public async Task<ActionResult<CategoryHeaderDto>> DeleteCategoryAsync([FromRoute] int id, CancellationToken cancellationToken = default)
	{
		var command = new DeleteCategoryCommand { InvokerId = requestContextProvider.InvokerId, CategoryId = id };
		var category = await categoryService.DeleteCategoryAsync(command, cancellationToken);
		return Ok(category);
	}
}