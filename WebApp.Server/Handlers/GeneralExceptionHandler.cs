using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Server.Handlers;

public class GeneralExceptionHandler(ILogger<GeneralExceptionHandler> logger) : IExceptionHandler
{
	private readonly ILogger<GeneralExceptionHandler> logger = logger;
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		var message = "An unexpected error occurred.";
		logger.LogError(exception, message);

		var problemDetails = new ProblemDetails
		{
			Type = $"https://httpstatuses.com/{StatusCodes.Status500InternalServerError}",
			Title = "Internal Server Error",
			Status = StatusCodes.Status500InternalServerError,
			Detail = message,
			Instance = httpContext.Request.Path,
		};

		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
		logger.LogInformation(exception, "A general exception occurred and got caught: {Message}", exception.Message);
		return true;
	}
}