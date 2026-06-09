using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Dal.Entities;

public class Limit : BaseEntity
{

	[Range(0.01, double.MaxValue)]
	[Precision(18, 2)]
	required public decimal MaxAmount { get; set; }
	[Range(0.01, double.MaxValue)]
	[Precision(18, 2)]
	required public decimal CurrentAmount { get; set; }
	public string? Description { get; set; }
	required public DateOnly StartDate { get; set; }
	required public DateOnly EndDate { get; set; }
	required public bool IsActive { get; set; }
	required public bool IsDeleted { get; set; }
	required public bool IncludeIncome { get; set; }

	required public int UserId { get; set; }
	public User			User   { get; set; } = null!;
}