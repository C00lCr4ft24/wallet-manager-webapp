
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Service.Exceptions;

namespace WebApp.Server.Handlers;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
	private readonly ILogger<ApiExceptionHandler> logger = logger;
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		if(exception is not ApiException apiException) return false;

		httpContext.Response.StatusCode = apiException.StatusCode;

		var problemDetails = new ProblemDetails
		{
			Type = $"https://httpstatuses.com/{apiException.StatusCode}",
			Title = apiException.StatusCodeTitle,
			Status = apiException.StatusCode,
			Detail = apiException.ExceptionMessage,
			Instance = httpContext.Request.Path,
			Extensions =
			{
				["reason"] = apiException.ReasonMessage
			}
		};

		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
		logger.LogInformation(exception, "An API exception occurred and got caught: {Message}", exception.Message);
		return true;
	}
}