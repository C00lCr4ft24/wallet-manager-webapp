namespace WebApp.Service.Commands;

public record CreateWalletCommand
{
	required public int InvokerId { get; init; }
	required public string Name { get; init; } = null!;
	required public decimal Balance { get; init; }
};
public record GetWalletsCommand
{
	required public int InvokerId { get; init; }
	required public int UserId { get; init; }
	required public bool LoadUsers { get; init; }
	required public bool LoadTransactions { get; init; }
};
public record GetWalletCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
	required public bool LoadUsers { get; init; }
	required public bool LoadTransactions { get; init; }
};
public record UpdateWalletCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
	required public string Name { get; init; } = null!;
	required public decimal Balance { get; init; }
};
public record DeleteWalletCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
};

public record AddUserToWalletCommand
{
	required public int InvokerId { get; init; }
	required public int WalletId { get; init; }
	required public int UserId { get; init; }
	required public bool IsOwner { get; init; }
};
public record GetWalletUserCommand
{
	required public int InvokerId { get; init; }
	required public int UserId { get; init; }
}
public record RemoveUserFromWalletCommand
{
	required public int InvokerId { get; init; }
	required public int WalletId { get; init; }
	required public int UserId { get; init; }
};
public record ChangeUserRoleInWalletCommand
{
	required public int InvokerId { get; init; }
	required public int WalletId { get; init; }
	required public int UserId { get; init; }
	required public bool IsOwner { get; init; }
};