using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces;

public interface ICategoryService
{
	Task<CategoryHeaderDto> CreateCategoryAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default);
	Task<CategoryHeaderDto> GetCategoryAsync(GetCategoryCommand command, CancellationToken cancellationToken = default);
	Task<IEnumerable<CategoryHeaderDto>> GetCategoriesAsync(GetCategoriesCommand command, CancellationToken cancellationToken = default);
	Task<CategoryHeaderDto> UpdateCategoryAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default);
	Task<CategoryHeaderDto> DeleteCategoryAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default);
}