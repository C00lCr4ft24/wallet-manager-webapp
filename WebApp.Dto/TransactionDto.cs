using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Response]
public record TransactionHeaderDto
{
	required public int Id { get; init; }
	required public string Name { get; init; } = string.Empty;
	required public string Description { get; init; } = string.Empty;
	required public decimal Amount { get; init; }
	required public DateTimeOffset Date { get; init; }
	required public DateTimeOffset CreatedAt { get; init; }
	required public UserMinimalDto CreatedByUser { get; init; }
	required public CategoryHeaderDto? Category { get; init; } 
};

[Response]
public record TransactionDataDto : TransactionHeaderDto
{
	required public WalletHeaderDto Wallet { get; init; } = null!;
};

[Request]
public record CreateTransactionDto
{
	public int? WalletId { get; init; }
	public int? CategoryId { get; init; }
	required public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	required public decimal Amount { get; init; }
	required public DateTimeOffset Date { get; init; }
};

[Request] //WalletId from route
public record UpdateTransactionDto
{
	public int? WalletId { get; init; }
	public int? CategoryId { get; init; }
	required public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	required public decimal Amount { get; init; }
	required public DateTimeOffset Date { get; init; }
};

[Request]
public record GetAllTransactionsDto
{
	public IEnumerable<int>? WalletIds { get; init; }
	public IEnumerable<int>? CategoryIds { get; init; }
	public DateTime? startDate { get; init; }
	public DateTime? endDate { get; init; }
}