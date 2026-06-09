namespace WebApp.Dal.Entities;

public class FamilyUser : BaseEntity
{
	required public int UserId { get; set; }
	public User User { get; set; } = null!;
	required public int FamilyId { get; set; }
	public Family Family { get; set; } = null!;
	required public bool IsOwner { get; set; }
	required public DateTimeOffset JoinedAt { get; set; }
}
