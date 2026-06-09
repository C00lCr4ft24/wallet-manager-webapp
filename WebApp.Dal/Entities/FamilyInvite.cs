
namespace WebApp.Dal.Entities;

public class FamilyInvite : BaseEntity
{
	required public bool IsAccepted { get; set; }
	required public bool IsAnswered { get; set; }
	required public int FamilyId { get; set; }
	required public int InviterId { get; set; }
	required public int InviteeId { get; set; }
	
	public Family Family { get; set; } = null!;
	public User Inviter { get; set; } = null!;
	public User Invitee { get; set; } = null!;
}