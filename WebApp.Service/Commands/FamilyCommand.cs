namespace WebApp.Service.Commands;

public record CreateFamilyCommand
{
	required public int InvokerId { get; init; }
	required public string Name { get; init; }
}
public record GetFamilyCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
}
public record UpdateFamilyCommand
{
	required public int InvokerId { get; init; }
	required public string Name { get; init; }
}
public record DeleteFamilyCommand
{
	required public int InvokerId { get; init; }
}
public record AddUserToFamilyCommand
{
	required public int InvokerId { get; init; }
	required public string UserEmail { get; init; }
}

public record GetFamilyUsersCommand
{
	required public int InvokerId { get; init; }
}

public record GetCurrentFamilyUserCommand
{
	required public int InvokerId { get; init; }
}

public record GetFamilyUserCommand
{
	required public int InvokerId { get; init; }
	required public int FamilyUserId { get; init; }
}

public record UpdateFamilyUserCommand
{
	required public int InvokerId { get; init; }
	required public int FamilyUserId { get; init; }
	required public bool IsOwner { get; init; }
}