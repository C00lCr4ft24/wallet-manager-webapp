
namespace WebApp.Dto;

public record WalletUserDto
{
	required public int Id { get; init; }
	public UserMinimalDto User { get; init; } = null!;
	public WalletHeaderDto Wallet { get; init; } = null!;
	required public bool IsOwner { get; init; }
	required public DateTimeOffset JoinedAt { get; init; }
	required public int UpdatedByUserId { get; init; }
}