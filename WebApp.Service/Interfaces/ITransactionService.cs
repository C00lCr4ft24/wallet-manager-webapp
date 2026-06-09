using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces
{
	public interface ITransactionService
	{
		Task<TransactionDataDto> CreateTransactionAsync(CreateTransactionCommand command, CancellationToken cancellationToken  = default);
		Task<TransactionDataDto> GetTransactionAsync(GetTransactionCommand command, CancellationToken cancellationToken  = default);
		Task<TransactionDataDto> UpdateTransactionAsync(UpdateTransactionCommand command, CancellationToken cancellationToken  = default);
		Task<TransactionDataDto> DeleteTransactionAsync(DeleteTransactionCommand command, CancellationToken cancellationToken = default);
		Task<IEnumerable<TransactionDataDto>> GetAllTransactionsAsync(GetAllTransactionsCommand command, CancellationToken cancellationToken = default);
	}
}