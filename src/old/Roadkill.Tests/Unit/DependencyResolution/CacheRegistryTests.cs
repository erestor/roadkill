using System.Runtime.Caching;
using NUnit.Framework;
using Roadkill.Core.Cache;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;

namespace Roadkill.Tests.Unit.DependencyResolution
{
    [TestFixture]
	[Category("Unit")]
	public class CacheRegistryTests : RegistryTestsBase
    {
        [Test]
		public void should_register_cache_types()
		{
			// Arrange + Act + Assert
			AssertDefaultType<ListCache, ListCache>();
			AssertDefaultType<PageViewModelCache, PageViewModelCache>();
			AssertDefaultType<ObjectCache, MemoryCache>();
			AssertDefaultType<IPluginCache, SiteCache>();
		}

		[Test]
		public void object_cache_should_be_singleton_roadkill_memorycache()
		{
			// Arrange
			var container = Container;

			// Act
			var objectCache1 = container.GetInstance<ObjectCache>();
			var objectCache2 = container.GetInstance<ObjectCache>();

			// Assert
			Assert.That(objectCache1, Is.EqualTo(objectCache2));

			MemoryCache memoryCache = objectCache1 as MemoryCache;
			Assert.That(memoryCache, Is.Not.Null);
			Assert.That(memoryCache.Name, Is.EqualTo("Roadkill"));
		}

		[Test]
		public void cache_types_should_be_singletons()
		{
			// Arrange
			var container = Container;

			// Act
			var listCache1 = container.GetInstance<ListCache>();
			var listCache2 = container.GetInstance<ListCache>();

			var siteCache1 = container.GetInstance<SiteCache>();
			var siteCache2 = container.GetInstance<SiteCache>();

			var pageViewModelCache1 = container.GetInstance<PageViewModelCache>();
			var pageViewModelCache2 = container.GetInstance<PageViewModelCache>();

			// Assert
			Assert.That(listCache1, Is.EqualTo(listCache2));
			Assert.That(siteCache1, Is.EqualTo(siteCache2));
			Assert.That(pageViewModelCache1, Is.EqualTo(pageViewModelCache2));
		}
	}
}
