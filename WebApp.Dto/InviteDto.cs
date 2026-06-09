using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Request]
public record CreateFamilyInviteDto
{
	required public string UserEmail { get; init; }
}
[Request]
public record CreateWalletInviteDto
{
	required public int UserId { get; init; }
}
[Request]
public record RespondToInviteDto
{
	required public int Id { get; init; }
	required public bool Accept { get; init; }
}
[Response]
public record FamilyInviteDto
{
	public int Id { get; init; }
	public FamilyHeaderDto Family { get; init; } = null!;
	public UserHeaderDto InviterUser { get; init; } = null!;
	public UserHeaderDto InvitedUser { get; init; } = null!;
	public DateTimeOffset CreatedAt { get; init; }
	public bool IsAccepted { get; init; }
}
[Response]
public record WalletInviteDto
{
	public int Id { get; init; }
	public WalletHeaderDto Wallet { get; init; } = null!;
	public UserHeaderDto InviterUser { get; init; } = null!;
	public UserHeaderDto InvitedUser { get; init; } = null!;
	public DateTimeOffset CreatedAt { get; init; }
	public bool IsAccepted { get; init; }
}