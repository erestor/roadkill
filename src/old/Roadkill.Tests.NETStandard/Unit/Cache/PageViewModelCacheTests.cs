using Roadkill.Core.Cache;
using Roadkill.Core.Configuration;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Tests.NETStandard.NUnitToXUnit;

namespace Roadkill.Tests.NETStandard.Unit.Cache
{
	/// <summary>
	/// Most of the PageViewModelCache tests are in the pageservicetests, to test
	/// the integration between the cache and the pageservice.
	/// </summary>
	[Category("Unit")]
	public class PageViewModelCacheTests
	{
		[Test]
		public void removeall_should_remove_pageviewmodelcache_keys_only()
		{
			// Arrange
			CacheMock cache = new CacheMock();
			cache.Add("site.blah", "xyz", new CacheItemPolicy());

			ApplicationSettings settings = new ApplicationSettings() { UseObjectCache = false };
			PageViewModelCache pageCache = new PageViewModelCache(settings, cache);
			pageCache.Add(1, 1, new PageViewModel());

			// Act
			pageCache.RemoveAll();

			// Assert
			Assert.That(cache.Count(), Is.EqualTo(1));
		}
	}
}