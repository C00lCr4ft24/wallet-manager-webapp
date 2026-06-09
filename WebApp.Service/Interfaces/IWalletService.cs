using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces
{
	public interface IWalletService
	{
		Task<WalletDataDto> ChangeUserRoleInWalletAsync(ChangeUserRoleInWalletCommand command, CancellationToken cancellationToken = default);
		Task<WalletInviteDto> CreateInviteAsync(CreateWalletInviteCommand command, CancellationToken cancellationToken = default);
		Task<WalletHeaderDto> CreateWalletAsync(CreateWalletCommand command, CancellationToken cancellationToken = default);
		Task<WalletHeaderDto> DeleteWalletAsync(DeleteWalletCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<WalletInviteDto>> GetReceivedInvitesForWalletAsync(GetWalletInvitesCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<WalletInviteDto>> GetSentInvitesForWalletAsync(GetWalletInvitesCommand command, CancellationToken cancellationToken = default);
		Task<WalletDataDto> GetWalletAsync(GetWalletCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<WalletDataDto>> GetWalletsAsync(GetWalletsCommand command, CancellationToken cancellationToken = default);
		Task<WalletDataDto> RemoveUserFromWalletAsync(RemoveUserFromWalletCommand command, CancellationToken cancellationToken = default);
		Task<WalletUserDto> RespondToInviteAsync(RespondToInviteCommand command, CancellationToken cancellationToken = default);
		Task<WalletHeaderDto> UpdateWalletAsync(UpdateWalletCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<WalletInviteDto>> GetAllSentWalletInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<WalletInviteDto>> GetAllReceivedWalletInvitesAsync(GetInvitesCommand command, CancellationToken cancellationToken = default);
		Task<WalletUserDto> GetWalletUserAsync(GetWalletUserCommand command, CancellationToken cancellationToken = default);
	}
}