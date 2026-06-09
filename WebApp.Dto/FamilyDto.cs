namespace WebApp.Dto;

public record FamilyHeaderDto
{
	public int Id { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public bool IsShared { get; init; }
	public string Name { get; init; } = string.Empty;
}

public record FamilyDataDto : FamilyHeaderDto
{
	public IEnumerable<FamilyUserDto> Users { get; init; } = [];
	public IEnumerable<LimitDto> Limits { get; init; } = [];
	public IEnumerable<CategoryHeaderDto> Categories { get; init; } = [];
}