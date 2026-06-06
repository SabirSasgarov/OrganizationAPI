using OriganizationAPI.Handler;
using System.Net;
using System.Text.Json;

namespace OriganizationAPI.Middlewares
{
	public class ExceptionHandlingMidlleware(RequestDelegate next)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				ResponseHandler<bool> responseHandler = ResponseHandler<bool>.FailureResponse([ex.Message]);
				context.Response.ContentType = "application/json";
				var serializedResponse = JsonSerializer.Serialize(responseHandler);
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				await context.Response.WriteAsync(serializedResponse);
			}
		}
		//private Task HandleExceptionAsync(HttpContext context, Exception ex)
		//{
		//	context.Response.ContentType = "application/json";
		//	context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		//	var result = JsonSerializer.Serialize(new { Error = ex.Message });
		//	return context.Response.WriteAsync(result);
		//}
	}
}
