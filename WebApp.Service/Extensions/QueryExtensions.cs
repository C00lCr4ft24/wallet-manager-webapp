
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApp.Service.Exceptions;

namespace WebApp.Service.Extensions;

public static class QueryExtensions
{
	public static async Task<T> SingleOrNotFoundAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, string attributeName, object attributeValue, CancellationToken cancellationToken = default)
	{
		try 
		{
			return await query.SingleAsync(predicate, cancellationToken);
		}
		catch (InvalidOperationException)
		{
			var entityName = typeof(T).Name;
			throw new NotFoundException(
				exceptionMsg: Messages.NotFound(entityName),
				reasonMsg: Messages.DoesNotExist(entityName, attributeName, attributeValue)
				);
		}
	}
}