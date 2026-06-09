namespace WebApp.Dal.Entities;

public class WalletUser : BaseEntity
{
	required public int WalletId { get; set; }
	public Wallet Wallet { get; set; } = null!;
	required public int UserId { get; set; }
	public User User { get; set; } = null!;
	required public bool IsOwner { get; set; }
	required public DateTimeOffset JoinedAt { get; set; }
}