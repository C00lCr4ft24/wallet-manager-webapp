using WebApp.Dal.Entities;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces
{
	public interface IValidationService
	{
		void ValidateTransaction(Transaction transaction);
		void ValidateWallet(Wallet wallet);
		void ValidateEmail(string email);
		void ValidateCategory(Category category);
		void ValidateLimit(Limit limit);
		void ValidateColor(string color);
	}
}