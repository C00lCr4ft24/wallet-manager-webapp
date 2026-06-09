using Microsoft.AspNetCore.Identity;

namespace WebApp.Dal.Entities;

public class User : IdentityUser<int>, IBaseEntity
{
	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
	public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
	public int CreatedByUserId { get; set; }
	public int UpdatedByUserId { get; set; }
	public string Name { get; set; } = null!;
	public DateTime? DateOfBirth { get; set; }
	public int FamilyUserId { get; set; }
	public FamilyUser FamilyUser { get; set; } = null!;
	public ICollection<WalletUser> WalletUsers { get; set; } = new HashSet<WalletUser>();
	public ICollection<Limit> Limits { get; set; } = new HashSet<Limit>();
	//FAMILY INVITES
	public ICollection<FamilyInvite> SentFamilyInvites { get; set; } = new HashSet<FamilyInvite>();
	public ICollection<FamilyInvite> ReceivedFamilyInvites { get; set; } = new HashSet<FamilyInvite>();
	//WALLET INVITES
	public ICollection<WalletInvite> SentWalletInvites { get; set; } = new HashSet<WalletInvite>();
	public ICollection<WalletInvite> ReceivedWalletInvites { get; set; } = new HashSet<WalletInvite>();
}