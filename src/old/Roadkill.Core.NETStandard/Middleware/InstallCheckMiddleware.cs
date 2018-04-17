using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Roadkill.Core.Configuration;

namespace Roadkill.Core.Owin
{
	// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware
	public class InstallCheckMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ApplicationSettings _appSettings;

		public InstallCheckMiddleware(RequestDelegate next, ApplicationSettings appSettings)
		{
			_next = next;
			_appSettings = appSettings;
		}

		public async Task Invoke(HttpContext context)
		{
			var appSettings = _appSettings;
			if (appSettings.Installed == false && IsOnInstallPage(context) == false && IsHtmlRequest(context))
			{
				context.Response.Redirect("/Install/");
			}
			else
			{
				await _next.Invoke(context);
			}
		}

		private static bool IsHtmlRequest(HttpContext context)
		{
			return !string.IsNullOrEmpty(context.Request.Headers["Accept"]) && context.Request.Headers["Accept"].Equals("text/html");
		}

		private bool IsOnInstallPage(HttpContext context)
		{
			return context.Request.Path.Value.StartsWith("/Install/");
		}
	}
}