using System;
using System.Threading.Tasks;
using System.Web.Configuration;
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
				context.Response.Redirect(_appBaseUri + "Install/");
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
			return context.Request.Uri.PathAndQuery.StartsWith(_appBaseUri + "Install/", StringComparison.InvariantCultureIgnoreCase);
		}

		readonly string _appBaseUri = WebConfigurationManager.AppSettings["AppBaseUri"];
	}
}