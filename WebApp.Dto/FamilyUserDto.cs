using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Response]
public record FamilyUserDto
{
	required public int Id { get; init; }
	required public UserMinimalDto User { get; init; }
	required public FamilyHeaderDto Family { get; init; }
	required public bool IsOwner { get; init; }
	required public DateTimeOffset JoinedAt { get; init; }
	required public int UpdatedByUserId { get; init; }
}
