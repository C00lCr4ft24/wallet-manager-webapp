
using WebApp.Dal.Entities;

namespace WebApp.Service.Commands;
public record CreateLimitCommand
{
	required public int InvokerId { get; init; }
	required public int UserId { get; init; }
	required public decimal MaxAmount { get; init; }
	required public DateOnly StartDate { get; init; }
	required public DateOnly EndDate { get; init; }
	required public bool IsActive { get; init; }
	required public bool IncludeIncome { get; init; }
	public string? Description { get; init; }
};
public record UpdateLimitCommand
{
	required public int InvokerId { get; init; }
	required public int LimitId { get; init; }
	public decimal? MaxAmount { get; init; }
	public DateOnly? StartDate { get; init; }
	public DateOnly? EndDate { get; init; }
	public bool? IsActive { get; init; }
	public bool? IncludeIncome { get; init; }
	public string? Description { get; init; }
};
public record GetLimitByIdCommand
{
	required public int InvokerId { get; init; }
	required public int LimitId { get; init; }
	required public bool LoadFamily { get; init; }
	required public bool LoadUser { get; init; }
};
public record GetAllLimitsCommand
{
	required public int InvokerId { get; init; }
	public int? UserId { get; init; }
};

public record DeleteLimitCommand
{
	required public int InvokerId { get; init; }
	required public int LimitId { get; init; }
};