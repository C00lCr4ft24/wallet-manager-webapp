namespace WebApp.Service.Interfaces
{
	public interface IAuthorizationService
	{
		Task DoesUserHaveAccessToCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default);
	}
}