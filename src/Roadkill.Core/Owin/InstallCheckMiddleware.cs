using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Roadkill.Core.Configuration;

namespace Roadkill.Core.Owin
{
	public class InstallCheckMiddleware : OwinMiddleware
	{
		private readonly ApplicationSettings _appSettings;

		public InstallCheckMiddleware(OwinMiddleware next, ApplicationSettings appSettings) : base(next)
		{
			_appSettings = appSettings;
		}

		public override async Task Invoke(IOwinContext context)
		{
			var appSettings = _appSettings;
			if (appSettings.Installed == false && IsOnInstallPage(context) == false && IsHtmlRequest(context))
			{
#if DEBUG
				context.Response.Redirect("/Roadkill.Web/Install/");
#else
				context.Response.Redirect("/wiki/Install/");
#endif
			}
			else
			{
				await Next.Invoke(context);
			}
		}

		private static bool IsHtmlRequest(IOwinContext context)
		{
			return !string.IsNullOrEmpty(context.Request.Accept) && context.Request.Accept.Contains("text/html");
		}

		private bool IsOnInstallPage(IOwinContext context)
		{
#if DEBUG
			return context.Request.Uri.PathAndQuery.StartsWith("/Roadkill.Web/Install/", StringComparison.InvariantCultureIgnoreCase);
#else
			return context.Request.Uri.PathAndQuery.StartsWith("/wiki/Install/", StringComparison.InvariantCultureIgnoreCase);
#endif
		}
	}
}