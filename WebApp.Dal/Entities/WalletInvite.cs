using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Dal.Entities;

public class WalletInvite : BaseEntity
{
	required public bool IsAccepted { get; set; }
	required public bool IsAnswered { get; set; }
	required public int WalletId { get; set; }
	required public int InviterId { get; set; }
	required public int InviteeId { get; set; }

	public Wallet Wallet { get; set; } = null!;
	public User Inviter { get; set; } = null!;
	public User Invitee { get; set; } = null!;
}