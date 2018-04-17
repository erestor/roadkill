using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Roadkill.Core.Plugins;
using PluginSettings = Roadkill.Core.Plugins.Settings;

namespace Roadkill.Core.Cache
{
	/// <summary>
	/// Caches items used across the entire Roadkill site.
	/// </summary>
	public class SiteCache : IPluginCache
	{
		private readonly IMemoryCache _cache;
		private readonly HashSet<string> _cacheKeys;

		/// <summary>
		/// Initializes a new instance of the <see cref="SiteCache"/> class.
		/// </summary>
		/// <param name="cache">The underlying OjectCache - a MemoryCache by default.</param>
		public SiteCache(IMemoryCache cache)
		{
			_cache = cache;
			_cacheKeys = new HashSet<string>();
		}

		private void StoreKey(string key)
		{
			if (!_cacheKeys.Contains(key))
			{
				_cacheKeys.Add(key);
			}
		}

		/// <summary>
		/// Adds the navigation menu HTML to the cache.
		/// </summary>
		/// <param name="html">The menu's HTML.</param>
		public void AddMenu(string html)
		{
			string key = CacheKeys.MenuKey();
			StoreKey(key);

			ICacheEntry entry = _cache.CreateEntry(key);
			entry.SetValue(html);
		}

		/// <summary>
		/// Adds the navigation menu HTML for logged in users to the cache.
		/// </summary>
		/// <param name="html">The menu's HTML.</param>
		public void AddLoggedInMenu(string html)
		{
			string key = CacheKeys.LoggedInMenuKey();
			StoreKey(key);

			ICacheEntry entry = _cache.CreateEntry(key);
			entry.SetValue(html);
		}

		/// <summary>
		/// Adds the admin menu HTML to the cache.
		/// </summary>
		/// <param name="html">The menu's HTML.</param>
		public void AddAdminMenu(string html)
		{
			string key = CacheKeys.AdminMenuKey();
			StoreKey(key);

			ICacheEntry entry = _cache.CreateEntry(key);
			entry.SetValue(html);
		}

		/// <summary>
		/// Retrieves the navigation menu HTML from the cache, or null if it doesn't exist.
		/// </summary>
		/// <returns>The cache HTML for the menu.</returns>
		public string GetMenu()
		{
			return _cache.Get(CacheKeys.MenuKey()) as string;
		}

		/// <summary>
		/// Retrieves the navigation menu HTML for logged in users from the cache, or null if it doesn't exist.
		/// </summary>
		/// <returns>The cache HTML for the menu.</returns>
		public string GetLoggedInMenu()
		{
			return _cache.Get(CacheKeys.LoggedInMenuKey()) as string;
		}

		/// <summary>
		/// Retrieves the admin menu HTML from the cache, or null if it doesn't exist.
		/// </summary>
		/// <returns>The cache HTML for the admin menu.</returns>
		public string GetAdminMenu()
		{
			return _cache.Get(CacheKeys.AdminMenuKey()) as string;
		}

		/// <summary>
		/// Gets the plugin settings.
		/// </summary>
		/// <param name="plugin">The text plugin.</param>
		/// <returns>
		/// Returns the <see cref="Settings" /> or null if the plugin is not in the cache.
		/// </returns>
		public PluginSettings GetPluginSettings(TextPlugin plugin)
		{
			return _cache.Get(CacheKeys.PluginSettingsKey(plugin)) as PluginSettings;
		}

		/// <summary>
		/// Updates the plugin settings.
		/// </summary>
		/// <param name="plugin">The text plugin.</param>
		public void UpdatePluginSettings(TextPlugin plugin)
		{
			string key = CacheKeys.PluginSettingsKey(plugin);
			StoreKey(key);

			_cache.Remove(key);

			ICacheEntry entry = _cache.CreateEntry(key);
			entry.SetValue(plugin.Settings);
		}

		/// <summary>
		/// Removes the settings for given plugin from the cache.
		/// </summary>
		/// <param name="plugin">The plugin to remove the cached settings for.</param>
		public void RemovePluginSettings(TextPlugin plugin)
		{
			_cache.Remove(CacheKeys.PluginSettingsKey(plugin));
		}

		/// <summary>
		/// Removes all cached HTML for the navigation menus from the cache.
		/// </summary>
		public void RemoveMenuCacheItems()
		{
			_cache.Remove(CacheKeys.MenuKey());
			_cache.Remove(CacheKeys.LoggedInMenuKey());
			_cache.Remove(CacheKeys.AdminMenuKey());
		}

		/// <summary>
		/// Clears the underlying cache of the items the <see cref="SiteCache"/> manages.
		/// </summary>
		public void RemoveAll()
		{
			foreach (var key in GetAllPageKeys())
			{
				_cache.Remove(key);
			}
		}

		/// <summary>
		/// Returns all keys from the underlying cache the <see cref="SiteCache"/> manages
		/// </summary>
		/// <returns>A string list of the keys.</returns>
		public IEnumerable<string> GetAllPageKeys()
		{
			return _cacheKeys.Where(x => x.StartsWith(CacheKeys.SITE_CACHE_PREFIX))
				.OrderBy(x => x);
		}
	}
}