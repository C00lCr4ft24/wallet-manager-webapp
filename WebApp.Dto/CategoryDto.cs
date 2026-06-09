using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Response]
public record CategoryHeaderDto
{
	public int Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public string Icon { get; init; } = string.Empty;
	public string Color { get; init; } = string.Empty;
	public bool IsDefault { get; init; }
};

[Response]
public record CategoryDataDto : CategoryHeaderDto
{
	public IEnumerable<TransactionHeaderDto> Transactions { get; init; } = [];
};

[Request]
public record CreateCategoryDto
{
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	public string? Icon { get; init; }
	public string? Color { get; init; }
};
[Request]
public record UpdateCategoryDto
{
	public string? Name { get; init; }
	public string? Description { get; init; }
	public string? Icon { get; init; }
	public string? Color { get; init; }
};