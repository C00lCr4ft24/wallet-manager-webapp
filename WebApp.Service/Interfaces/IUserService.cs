using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces
{
	public interface IUserService
	{
		Task<UserHeaderDto> GetUserAsync(GetUserCommand command, CancellationToken cancellationToken = default);
		Task<UserHeaderDto> RegisterUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default);
		Task<UserHeaderDto> UpdateUserEmailAsync(UpdateUserEmailCommand command, CancellationToken cancellationToken = default);
		Task<UserHeaderDto> UpdateUserNameAsync(UpdateUserNameCommand command, CancellationToken cancellationToken = default);
		Task<UserHeaderDto> UpdateUserDateOfBirthAsync(UpdateUserDateOfBirthCommand command, CancellationToken cancellationToken = default);
		Task<UserHeaderDto> UpdateUserPasswordAsync(UpdateUserPasswordCommand command, CancellationToken cancellationToken = default);
		Task<UserHeaderDto> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken = default);

	}
}