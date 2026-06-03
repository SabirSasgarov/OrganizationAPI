using System.Net.Http.Headers;

namespace Organization.MVC.Handlers
{
	public class AuthTokenHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public AuthTokenHandler(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var httpContext = _httpContextAccessor.HttpContext;
			if(httpContext != null)
			{
				string? token = null;

				// First, check if token exists in HttpContext.Items
				if (httpContext.Items.ContainsKey("AuthToken"))
				{
					token = httpContext.Items["AuthToken"]?.ToString()!;
				}

				// If not found in Items, try to get it from the cookie
				if (string.IsNullOrEmpty(token) && httpContext.Request.Cookies.TryGetValue("AuthToken", out var cookieToken))
				{
					token = cookieToken;
				}

				// Add token to request header if found
				if (!string.IsNullOrEmpty(token))
				{
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}
			}
			return await base.SendAsync(request, cancellationToken);
		}
	}
}
