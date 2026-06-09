using System.ComponentModel.DataAnnotations;

namespace WebApp.Service.Commands;

public record CreateTransactionCommand
{
	required public int InvokerId { get; init; }
	required public string Name { get; init; } = null!;
	required public string? Description { get; init; }
	required public decimal Amount { get; init; }
	required public bool IsImplicit { get; init; }
	[Range(typeof(DateTimeOffset), "1900-01-01", "9999-12-31")]
	required public DateTimeOffset Date { get; init; }
	required public int WalletId { get; init; }
	required public int? CategoryId { get; init; }
}
public record GetTransactionCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
}
public record GetAllTransactionsCommand
{
	required public int InvokerId { get; init; }
	public IEnumerable<int>? CategoryIds { get; init; }
	public IEnumerable<int>? WalletIds { get; init; }
	public DateTime? StartDate { get; init; }
	public DateTime? EndDate { get; init; }
}
public record UpdateTransactionCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
	required public string? Name { get; init; }
	required public string? Description { get; init; }
	required public decimal? Amount { get; init; }
	required public DateTimeOffset? Date { get; init; }
	required public int? WalletId { get; init; }
	required public int? CategoryId { get; init; }
}
public record DeleteTransactionCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
}