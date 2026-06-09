using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Response]
public record WalletHeaderDto
{
	public int Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public decimal Balance { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public DateTimeOffset UpdatedAt { get; init; }
}

[Response]
public record WalletDataDto : WalletHeaderDto
{
	public IEnumerable<WalletUserDto> Users { get; init; } = [];
	public IEnumerable<TransactionHeaderDto> Transactions { get; init; } = [];
};

[Request]
public record CreateWalletDto
{
	public string Name { get; init; } = string.Empty;
	public decimal Balance { get; init; }
};

[Request]
public record UpdateWalletDto
{
	public string Name { get; init; } = string.Empty;
	public decimal Balance { get; init; }
};

[Request]
public record AddUserToWalletDto
{
	public int UserId { get; init; }
};

[Request]
public record ChangeUserRoleInWalletDto
{
	public bool IsOwner { get; init; }
};