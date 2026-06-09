using WebApp.Dto.Attributes;

namespace WebApp.Dto;

[Response]
public record UserMinimalDto
{
	public int Id { get; init; }
	public string UserName { get; init; } = string.Empty;
}

[Response]
public record UserHeaderDto : UserMinimalDto
{
	public string Email { get; init; } = string.Empty;
	public DateTime DateOfBirth { get; init; }
	public decimal TotalBalance { get; init; }
}

[Response]
public record UserDataDto : UserHeaderDto
{
	public IEnumerable<WalletHeaderDto> WalletIds { get; init; } = [];
	public IEnumerable<LimitDto> LimitIds { get; init; } = [];
}

[Request]
public record RegisterUserDto
{
	public string Email { get; init; } = string.Empty;
	public string Password { get; init; } = string.Empty;
};

[Request]
public record LoginUserDto	
{
	public string Email { get; init; } = string.Empty;
	public string Password { get; init; } = string.Empty;
};

[Request]
public record UpdateUserEmailDto
{
	public string Email { get; init; } = string.Empty;
};

[Request]
public record UpdateUserNameDto
{
	public string UserName { get; init; } = string.Empty;
};

[Request]
public record UpdateUserDateOfBirthDto
{
	public DateTime DateOfBirth { get; init; }
};

[Request]
public record UpdateUserPasswordDto
{
	public string OldPassword { get; init; } = string.Empty;
	public string NewPassword { get; init; } = string.Empty;
};

[Response]
public record LoginResponseDto
{
	public string Message { get; init; } = string.Empty;
	public string AccessToken { get; init; } = string.Empty;
};

[Response]
public record RefreshTokenResponseDto
{
	public string Message { get; init; } = string.Empty;
	public string AccessToken { get; init; } = string.Empty;
};
