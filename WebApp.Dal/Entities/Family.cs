namespace WebApp.Dal.Entities;

public class Family : BaseEntity
{
	required public bool IsShared { get; set; }
	required public string Name { get; set; }
	public ICollection<FamilyUser> FamilyUsers { get; set; } = new HashSet<FamilyUser>();
	public ICollection<Category> Categories { get; set; } = new HashSet<Category>();
	public ICollection<FamilyInvite> Invites { get; set; } = new HashSet<FamilyInvite>();
}