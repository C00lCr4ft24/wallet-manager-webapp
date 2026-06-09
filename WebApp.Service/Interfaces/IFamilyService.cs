using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces
{
	public interface IFamilyService
	{
		Task<FamilyInviteDto> CreateInviteAsync(CreateFamilyInviteCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<FamilyInviteDto>> GetReceivedInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<FamilyInviteDto>> GetSentInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default);
		Task<FamilyUserDto> RespondToInviteAsync(RespondToInviteCommand command, CancellationToken cancellationToken = default);
		Task<FamilyInviteDto> DeleteInviteAsync(DeleteInviteCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<FamilyUserDto>> GetFamilyUsersAsync(GetFamilyUsersCommand command, CancellationToken cancellationToken = default);
		Task<FamilyUserDto> GetCurrentFamilyUserAsync(GetCurrentFamilyUserCommand command, CancellationToken cancellationToken = default);
		Task<FamilyUserDto> GetFamilyUserAsync(GetFamilyUserCommand command, CancellationToken cancellationToken = default);
		Task<FamilyUserDto> UpdateFamilyUserAsync(UpdateFamilyUserCommand command, CancellationToken cancellationToken = default);
	}
}