using Microsoft.EntityFrameworkCore;

namespace WebApp.Dal.Entities;

public class Transaction : BaseEntity
{
	required public string Name { get; set; }
	public string? Description { get; set; }

	[Precision(18, 2)]
	required public decimal Amount { get; set; }
	required public bool IsImplicit { get; set; }
	public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

	required public int WalletId { get; set; }
	public Wallet Wallet { get; set; } = null!;

	public int? CategoryId { get; set; }
	public Category? Category { get; set; }

	public User CreatedByUser { get; set; } = null!;
}