using WebApp.Dal.Entities;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces
{
	public interface ICommonService
	{
		Task<bool> AreUsersInTheSameFamilyAsync(int userId1, int userId2, CancellationToken cancellationToken = default);
		Task<int> CreateFamilyAsync(CreateFamilyCommand command, CancellationToken cancellationToken = default);
		Task<int> CreateTransactionAsync(CreateTransactionCommand command, CancellationToken cancellationToken = default);
		Task<int> CreateWalletAsync(CreateWalletCommand command, CancellationToken cancellationToken = default);
		Task<Category> GetCategoryEntityAsync(int categoryId, bool loadTransactions = false, CancellationToken cancellationToken = default);
		Task<Family> GetFamilyEntityAsync(int familyId, bool loadUsers = false, bool loadCategories = false, CancellationToken cancellationToken = default);
		Task<int> GetFamilyIdOfUserAsync(int userId, CancellationToken cancellationToken = default);
		Task<FamilyInvite> GetFamilyInviteEntityAsync(int inviteId, bool loadFamily = false, bool loadInviter = false, bool loadInvitee = false, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetFamilyInviteIdsByFamilyIdAsync(int familyId, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetFamilyInviteIdsByInviteeIdAsync(int inviteeId, bool includeAnswered = false, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetFamilyInviteIdsByInviterIdAsync(int inviterId, bool includeAnswered = false, CancellationToken cancellationToken = default);
		Task<Transaction> GetTransactionEntityAsync(int transactionId, bool loadWallet = false, CancellationToken cancellationToken = default);
		Task<User> GetUserEntityAsync(int userId, bool loadFamily = false, bool loadWallets = false, bool loadLimits = false, CancellationToken cancellationToken = default);
		Task<int> GetUserIdAsync(int userId, CancellationToken cancellationToken = default);
		Task<int> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default);
		Task<Wallet> GetWalletEntityAsync(int walletId, bool loadUsers = false, bool loadTransactions = false, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetWalletIdsOfUserAsync(int userId, CancellationToken cancellationToken = default);
		Task<WalletInvite> GetWalletInviteEntityAsync(int inviteId, bool loadWallet = false, bool loadInviter = false, bool loadInvitee = false, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetWalletInviteIdsByWalletIdAsync(int walletId, bool includeAnswered = false, CancellationToken cancellationToken = default);
		Task<Limit> GetLimitEntityAsync(int limitId, bool loadUser = false, CancellationToken cancellationToken = default);
		Task<bool> IsDateInRangeAsync(DateTimeOffset date, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
		bool IsDateInRange(DateTimeOffset date, DateOnly startDate, DateOnly endDate);
		Task<FamilyUser> GetFamilyUserEntityAsync(int familyUserId, bool loadUser = false, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetAllSentWalletInviteIdsAsync(int inviterId, bool includeAnswered = false, CancellationToken cancellationToken = default);
		Task<IEnumerable<int>> GetAllReceivedWalletInviteIdsAsync(int inviteeId, bool includeAnswered = false, CancellationToken cancellationToken = default);
	}
}