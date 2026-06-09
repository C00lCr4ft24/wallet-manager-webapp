namespace WebApp.Service.Commands;

public record CreateUserCommand
{
	required public string Email { get; init; }
	required public string Password { get; init; }
}
public record LoginUserCommand
{
	required public string Email { get; init; }
	required public string Password { get; init; }
}
public record GetUserCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
};
public record UpdateUserEmailCommand
{
	required public int InvokerId { get; init; }
	required public string Email { get; init; } = null!;
};
public record UpdateUserNameCommand
{
	required public int InvokerId { get; init; }
	required public string UserName { get; init; } = null!;
};
public record UpdateUserDateOfBirthCommand
{
	required public int InvokerId { get; init; }
	required public DateTime DateOfBirth { get; init; }
};
public record UpdateUserPasswordCommand
{
	required public int InvokerId { get; init; }
	required public string OldPassword { get; init; } = null!;
	required public string NewPassword { get; init; } = null!;
};
public record DeleteUserCommand
{
	required public int InvokerId { get; init; }
	required public int Id { get; init; }
};