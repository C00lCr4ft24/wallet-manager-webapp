namespace WebApp.Service.Commands;

public record CreateCategoryCommand
{
	required public int InvokerId { get; init; }
	required public string Name { get; init; }
	public string? Description { get; init; }
	public string? Icon { get; init; }
	public string? Color { get; init; }
};
public record UpdateCategoryCommand
{
	required public int InvokerId { get; init; }
	required public int CategoryId { get; init; }
	public string? Name { get; init; }
	public string? Description { get; init; }
	public string? Icon { get; init; }
	public string? Color { get; init; }
};
public record GetCategoryCommand
{
	required public int InvokerId { get; init; }
	required public int CategoryId { get; init; }
};
public record GetCategoriesCommand
{
	required public int InvokerId { get; init; }
};
public record DeleteCategoryCommand
{
	required public int InvokerId { get; init; }
	required public int CategoryId { get; init; }
};