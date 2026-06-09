using System.ComponentModel.DataAnnotations;

namespace WebApp.Dal.Entities;

public class Category : BaseEntity
{
	required public string Name { get; set; }
	public string? Description { get; set; }
	public string? Icon { get; set; }
	[RegularExpression(@"^#[0-9a-fA-F]{6}$")]
	[MaxLength(7)]
	public string? Color { get; set; }
	required public bool IsDefault { get; set; }
	public int?		FamilyId { get; set; }
	public Family?	Family { get; set; }
	public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
}