namespace WebApp.Dal.Entities;

public abstract class BaseEntity : IBaseEntity
{
	public int Id { get; set; }
	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
	public int CreatedByUserId { get; set; }
	public int UpdatedByUserId { get; set; }
}