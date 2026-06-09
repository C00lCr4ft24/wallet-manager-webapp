
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;

namespace WebApp.Service.Utilities;

public class AuthorizationService(ICommonService commonService) : IAuthorizationService
{
	private readonly ICommonService commonSrv = commonService;
	public async Task DoesUserHaveAccessToCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default)
	{
		var category = await commonSrv.GetCategoryEntityAsync(categoryId, cancellationToken: cancellationToken);
		var user = await commonSrv.GetUserEntityAsync(userId, loadFamily: true, cancellationToken: cancellationToken);
		if(category.FamilyId != null && user.FamilyUser.FamilyId != category.FamilyId)
		{
			throw new ForbiddenException(Messages.FailedToAccess(category), $"User with email '{user.Email}' is not member of category's family!");
		}
	}

	public async Task DoesUserHaveAccessToFamilyAsync(int userId, int familyId, CancellationToken cancellationToken = default)
	{
		var user = await commonSrv.GetUserEntityAsync(userId, loadFamily: true, cancellationToken: cancellationToken);
		if(user.FamilyUser.FamilyId != familyId)
		{
			throw new ForbiddenException(Messages.FailedToAccess(user.FamilyUser.Family), $"User with email '{user.Email}' is not member of family!");
		}
	}

	public async Task DoesUserHaveAccessToFamilyInviteAsync(int userId, int familyInviteId, CancellationToken cancellationToken = default)
	{
		var familyInvite = await commonSrv.GetFamilyInviteEntityAsync(familyInviteId, cancellationToken: cancellationToken);
		var user = await commonSrv.GetUserEntityAsync(userId, loadFamily: true, cancellationToken: cancellationToken);
		if(user.FamilyUser.FamilyId != familyInvite.FamilyId
			&& (familyInvite.InviteeId != userId || familyInvite.InviterId != userId))
		{
			throw new ForbiddenException(Messages.FailedToAccess(familyInvite), $"User with email '{user.Email}' is not member of family invite's family!");
		}
	}
}