
using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Interfaces;

public interface ILimitService
{
	Task<LimitDto> CreateLimitAsync(CreateLimitCommand command, CancellationToken cancellationToken = default);
	Task<LimitDto> GetLimitAsync(GetLimitByIdCommand command, CancellationToken cancellationToken = default);
	Task<LimitDto> UpdateLimitAsync(UpdateLimitCommand command, CancellationToken cancellationToken = default);
	Task<LimitDto> DeleteLimitAsync(DeleteLimitCommand command, CancellationToken cancellationToken = default);
	Task<IEnumerable<LimitDto>> GetAllLimitsAsync(GetAllLimitsCommand command, CancellationToken cancellationToken = default);
}