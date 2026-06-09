using WebApp.Dal.Entities;

namespace WebApp.Service.Exceptions;

public static class Messages
{
	public static string NotFound(string objectName) => $"{objectName} was not found!";
	public static string DoesNotExist(string objectName, string attributeName, object attributeValue) => $"{objectName} with {attributeName} '{attributeValue}' does not exist!";
	public static string FailedToValidate(IBaseEntity o) => $"Failed to validate {o.GetType().Name}!";
	public static string FailedToCreate(string objectName) => $"Failed to create {objectName}!";
	public static string FailedToAccess(IBaseEntity o) => $"Failed to access {o.GetType().Name}!";
	public static string FailedToUpdate(IBaseEntity o) => $"Failed to update {o.GetType().Name}!";
	public static string FailedToDelete(IBaseEntity o) => $"Failed to delete {o.GetType().Name}!";
}