namespace WebApp.Dal.Entities;

public interface IBaseEntity
{
	int Id { get; set; }
	DateTimeOffset CreatedAt { get; set; }
	DateTimeOffset UpdatedAt { get; set; }
	int CreatedByUserId { get; set; }
	int UpdatedByUserId { get; set; }
}
