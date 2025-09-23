using System.Net.Http.Headers;
using System.Security.Claims;

namespace AppView.Clients
{
	public class UserHeaderHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public UserHeaderHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var user = _httpContextAccessor.HttpContext?.User;
			var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!string.IsNullOrWhiteSpace(userId))
			{
				request.Headers.Remove("X-User-Id");
				request.Headers.Add("X-User-Id", userId);
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}


