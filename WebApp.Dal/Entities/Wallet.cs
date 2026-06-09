using Microsoft.EntityFrameworkCore;

namespace WebApp.Dal.Entities;

public class Wallet : BaseEntity
{
	required public string Name { get; set; }

	[Precision(18, 2)]
	required public decimal Balance { get; set; }

	public ICollection<WalletUser> WalletUsers { get; set; } = new HashSet<WalletUser>();
	public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
	public ICollection<WalletInvite> Invites { get; set; } = new HashSet<WalletInvite>();
}