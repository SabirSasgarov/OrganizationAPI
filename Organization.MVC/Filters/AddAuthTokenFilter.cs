using Microsoft.AspNetCore.Mvc.Filters;

namespace Organization.MVC.Filters
{
	public class AddAuthTokenFilter : IAsyncActionFilter
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public AddAuthTokenFilter(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var token = context.HttpContext.Request.Cookies["AuthToken"];
			if (string.IsNullOrEmpty(token))
			{
				context.HttpContext.Items["AuthToken"] = token;
			}
			await next();
		}
	}
}
