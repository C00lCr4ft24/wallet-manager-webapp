
namespace WebApp.Service.Commands;

public record CreateFamilyInviteCommand
{
	required public int InvokerId { get; init; }
	required public string UserEmail { get; init; }
}
public record CreateWalletInviteCommand
{
	required public int InvokerId { get; init; }
	required public int UserId { get; init; }
	required public int WalletId { get; init; }
}
public record GetInvitesCommand
{
	required public int InvokerId { get; init; }
	required public bool IncludeAnswered { get; init; }
}
public record GetWalletInvitesCommand : GetInvitesCommand
{
	required public int WalletId { get; init; }
}
public record RespondToInviteCommand
{
	required public int InvokerId { get; init; }
	required public int InviteId { get; init; }
	required public bool Accept { get; init; }
}
public record RespondToWalletInviteCommand : RespondToInviteCommand
{
	required public int WalletId { get; init; }
}
public record DeleteInviteCommand
{
	required public int InvokerId { get; init; }
	required public int InviteId { get; init; }
}