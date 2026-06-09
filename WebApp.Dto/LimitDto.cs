using WebApp.Dal.Entities;
using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Response]
public record LimitDto
{
	public int Id { get; init; }
	public UserHeaderDto User { get; init; } = null!;
	public string? Description { get; init; }
	public decimal MaxAmount { get; init; }
	public decimal CurrentAmount { get; init; }
	public DateOnly StartDate { get; init; }
	public DateOnly EndDate { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public int CreatedByUserId { get; init; }
	public bool IsActive { get; init; }
	public bool IsDeleted { get; init; }
	public bool IncludeIncome { get; init; }
};

[Request]
public record CreateLimitDto
{
	public int UserId { get; init; }
	public decimal MaxAmount { get; init; }
	public DateOnly StartDate { get; init; }
	public DateOnly EndDate { get; init; }
	public bool IncludeIncome { get; init; }
	public bool IsActive { get; init; }
	public string? Description { get; init; }
};

[Request]
public record GetLimitByIdDto
{
	public int Id { get; init; }
};
[Request]
public record UpdateLimitDto
{
	public decimal? MaxAmount { get; init; }
	public string? Description { get; init; }
	public DateOnly? StartDate { get; init; }
	public DateOnly? EndDate { get; init; }
	public bool? IsActive { get; init; }
	public bool? IncludeIncome { get; init; }
};