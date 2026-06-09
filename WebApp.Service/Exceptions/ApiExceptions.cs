using Microsoft.AspNetCore.Http;

namespace WebApp.Service.Exceptions;

public abstract class ApiException(string statusCodeTitle, int statusCode, string exceptionMsg, string reasonMsg = "") : Exception(exceptionMsg + reasonMsg)
{
	public int StatusCode { get; } = statusCode;
	public string StatusCodeTitle { get; } = statusCodeTitle;
	public string ExceptionMessage { get; } = exceptionMsg;
	public string ReasonMessage { get; } = reasonMsg;
}

public class BadRequestException(string exceptionMsg, string reasonMsg)
	: ApiException("Bad Request", StatusCodes.Status400BadRequest, exceptionMsg, reasonMsg)
{ }
public class UnauthorizedException(string exceptionMsg, string reasonMsg)
	: ApiException("Unauthorized", StatusCodes.Status401Unauthorized, exceptionMsg, reasonMsg)
{ }
public class ForbiddenException(string exceptionMsg, string reasonMsg)
	: ApiException("Forbidden", StatusCodes.Status403Forbidden, exceptionMsg, reasonMsg)
{ }
public class NotFoundException(string exceptionMsg, string reasonMsg)
	: ApiException("Not Found", StatusCodes.Status404NotFound, exceptionMsg, reasonMsg)
{ }
public class ConflictException(string exceptionMsg, string reasonMsg)
	: ApiException("Conflict", StatusCodes.Status409Conflict, exceptionMsg, reasonMsg)
{ }
public class InternalServerErrorException(string exceptionMsg, string reasonMsg)
	: ApiException("Internal Server Error", StatusCodes.Status500InternalServerError, exceptionMsg, reasonMsg)
{ }