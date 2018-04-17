using System;
using Roadkill.Core.Configuration;
using Roadkill.Core.Plugins;
using PluginSettings = Roadkill.Core.Plugins.Settings;

namespace Roadkill.Core.Database.Repositories
{
	public interface ISettingsRepository : IDisposable
	{
		void SaveSiteSettings(SiteSettings siteSettings);
		SiteSettings GetSiteSettings();
		void SaveTextPluginSettings(TextPlugin plugin);
		PluginSettings GetTextPluginSettings(Guid databaseId);
	}
}
