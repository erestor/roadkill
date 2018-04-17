using Microsoft.AspNetCore.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Mvc.Attributes;
using Roadkill.Core.Plugins;
using Roadkill.Core.Security;
using Roadkill.Core.Services;

namespace Roadkill.Core.Mvc.Controllers
{
	/// <summary>
	/// Provides functionality for /Special: urls
	/// </summary>
	[OptionalAuthorization]
	public class SpecialPagesController : ControllerBase
	{
		private IPluginFactory _pluginFactory;

		public SpecialPagesController(ApplicationSettings settings, UserServiceBase userManager, IUserContext context,
			SettingsService settingsService, IPluginFactory pluginFactory)
			: base(settings, userManager, context, settingsService)
		{
			_pluginFactory = pluginFactory;
		}

		/// <summary>
		/// Calls any special page plugin based on the id (the name).
		/// </summary>
		public ActionResult Index(string id)
		{
			SpecialPagePlugin plugin = _pluginFactory.GetSpecialPagePlugin(id);

			// Throw an HttpException so the customerrors is used and not the default asp.net 404 page
			if (plugin == null)
				return new StatusCodeResult(404);

			return plugin.GetResult(this);
		}
	}
}