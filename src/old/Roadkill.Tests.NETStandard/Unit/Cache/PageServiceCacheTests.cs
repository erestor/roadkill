using System;
using System.Collections.Generic;
using System.Linq;
using Markdig.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Roadkill.Core.Cache;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Mvc.Controllers;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Services;
using Roadkill.Core.Text.Parsers.Markdig;
using Roadkill.Core.Text.TextMiddleware;
using Roadkill.Tests.NETStandard.NUnitToXUnit;
using Roadkill.Tests.NETStandard.Unit.StubsAndMocks.Database;
using Roadkill.Tests.Unit.StubsAndMocks;
using Xunit;

namespace Roadkill.Tests.NETStandard.Unit.Cache
{
	[Trait("Category", "Unit")]
	public class PageServiceCacheTests
	{
		private PluginFactoryMock _pluginFactory;

		public PageServiceCacheTests()
		{
			_pluginFactory = new PluginFactoryMock();
		}

		public void getbyid_should_add_to_cache_when_pagesummary_does_not_exist_in_cache()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock pageModelCache = new CacheMock();
			PageService pageService = CreatePageService(pageModelCache, null, settingsRepository, pageRepository);

			PageViewModel expectedModel = CreatePageViewModel();
			expectedModel = pageService.AddPage(expectedModel); // get it back to update the version no.

			// Act
			pageService.GetById(1);

			// Assert
			CacheItem cacheItem = pageModelCache.CacheItems.First();
			string cacheKey = CacheKeys.PageViewModelKey(1, PageViewModelCache.LATEST_VERSION_NUMBER);
			NUnitAssert.That(cacheItem.Key, Is.EqualTo(cacheKey));

			PageViewModel actualModel = (PageViewModel)cacheItem.Value;
			NUnitAssert.That(actualModel.Id, Is.EqualTo(expectedModel.Id));
			NUnitAssert.That(actualModel.VersionNumber, Is.EqualTo(expectedModel.VersionNumber));
			NUnitAssert.That(actualModel.Title, Is.EqualTo(expectedModel.Title));
		}

		public void getbyid_should_load_from_cache_when_pagesummary_exists_in_cache()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock pageModelCache = new CacheMock();
			PageService pageService = CreatePageService(pageModelCache, null, settingsRepository, pageRepository);

			PageViewModel expectedModel = CreatePageViewModel();
			string cacheKey = CacheKeys.PageViewModelKey(1, PageViewModelCache.LATEST_VERSION_NUMBER);
			pageModelCache.Add(cacheKey, expectedModel, new CacheItemPolicy());

			// Act
			PageViewModel actualModel = pageService.GetById(1);

			// Assert
			NUnitAssert.That(actualModel.Id, Is.EqualTo(expectedModel.Id));
			NUnitAssert.That(actualModel.VersionNumber, Is.EqualTo(expectedModel.VersionNumber));
			NUnitAssert.That(actualModel.Title, Is.EqualTo(expectedModel.Title));
		}

		public void addpage_should_clear_list_and_pagesummary_caches()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock pageModelCache = new CacheMock();
			CacheMock listCache = new CacheMock();

			PageService pageService = CreatePageService(pageModelCache, listCache, settingsRepository, pageRepository);
			PageViewModel expectedModel = CreatePageViewModel();
			AddPageCacheItem(pageModelCache, "key", expectedModel);
			AddListCacheItem(listCache, "key", new List<string>() { "tag1", "tag2" });

			// Act
			pageService.AddPage(new PageViewModel() { Title = "totoro" });

			// Assert
			NUnitAssert.That(pageModelCache.CacheItems.Count, Is.EqualTo(0));
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(0));
		}

		public void AllPages_Should_Load_From_Cache(bool loadPageContent)
		{
			string cacheKey = (loadPageContent) ? (CacheKeys.AllPagesWithContent()) : (CacheKeys.AllPages());

			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock listCache = new CacheMock();

			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);
			PageViewModel expectedModel = CreatePageViewModel();
			AddListCacheItem(listCache, cacheKey, new List<PageViewModel>() { expectedModel });

			// Act
			IEnumerable<PageViewModel> actualList = pageService.AllPages(loadPageContent);

			// Assert
			NUnitAssert.That(actualList, Contains.Item(expectedModel));
		}

		public void AllPages_Should_Add_To_Cache_When_Cache_Is_Empty(bool loadPageContent)
		{
			// Arrange
			string cacheKey = (loadPageContent) ? (CacheKeys.AllPagesWithContent()) : (CacheKeys.AllPages());

			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Title = "1" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Title = "2" }, "text", "admin", DateTime.UtcNow);

			CacheMock listCache = new CacheMock();
			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);

			// Act
			pageService.AllPages(loadPageContent);

			// Assert
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(1));
			NUnitAssert.That(listCache.CacheItems.FirstOrDefault().Key, Is.EqualTo(cacheKey));
		}

		public void allpagescreatedby_should_load_from_cache()
		{
			string adminCacheKey = CacheKeys.AllPagesCreatedByKey("admin");
			string editorCacheKey = CacheKeys.AllPagesCreatedByKey("editor");

			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock listCache = new CacheMock();

			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);
			PageViewModel adminModel = CreatePageViewModel();
			PageViewModel editorModel = CreatePageViewModel("editor");
			listCache.Add(CacheKeys.AllPagesCreatedByKey("admin"), new List<PageViewModel>() { adminModel }, new CacheItemPolicy());
			listCache.Add(CacheKeys.AllPagesCreatedByKey("editor"), new List<PageViewModel>() { editorModel }, new CacheItemPolicy());

			// Act
			IEnumerable<PageViewModel> actualList = pageService.AllPagesCreatedBy("admin");

			// Assert
			NUnitAssert.That(actualList, Contains.Item(adminModel));
			NUnitAssert.That(actualList, Is.Not.Contains(editorModel));
		}

		public void allpagescreatedby_should_add_to_cache_when_cache_is_empty()
		{
			// Arrange
			string adminCacheKey = CacheKeys.AllPagesCreatedByKey("admin");

			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Title = "1" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Title = "2" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Title = "3" }, "text", "editor", DateTime.UtcNow);

			CacheMock listCache = new CacheMock();
			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);

			// Act
			pageService.AllPagesCreatedBy("admin");

			// Assert
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(1));
			NUnitAssert.That(listCache.CacheItems.FirstOrDefault().Key, Is.EqualTo(adminCacheKey));
		}

		public void alltags_should_load_from_cache()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock listCache = new CacheMock();

			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);
			List<string> expectedTags = new List<string>() { "tag1", "tag2", "tag3" };
			AddListCacheItem(listCache, CacheKeys.AllTags(), expectedTags);

			// Act
			IEnumerable<string> actualTags = pageService.AllTags().Select(x => x.Name);

			// Assert
			NUnitAssert.That(actualTags, Is.SubsetOf(expectedTags));
		}

		public void alltags_should_add_to_cache_when_cache_is_empty()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Tags = "tag1;tag2" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Tags = "tag3;tag4" }, "text", "admin", DateTime.UtcNow);

			CacheMock listCache = new CacheMock();
			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);

			// Act
			pageService.AllTags();

			// Assert
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(1));
			NUnitAssert.That(listCache.CacheItems.FirstOrDefault().Key, Is.EqualTo(CacheKeys.AllTags()));
		}

		public void deletepage_should_clear_list_and_pagesummary_caches()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page(), "text", "admin", DateTime.UtcNow);
			CacheMock pageCache = new CacheMock();
			CacheMock listCache = new CacheMock();

			PageService pageService = CreatePageService(pageCache, listCache, settingsRepository, pageRepository);
			PageViewModel expectedModel = CreatePageViewModel();
			AddPageCacheItem(pageCache, "key", expectedModel);
			AddListCacheItem(listCache, "key", new List<string>() { "tag1", "tag2" });

			// Act
			pageService.DeletePage(1);

			// Assert
			NUnitAssert.That(pageCache.CacheItems.Count, Is.EqualTo(0));
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(0));
		}

		public void findhomepage_should_load_from_cache()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock modelCache = new CacheMock();

			PageService pageService = CreatePageService(modelCache, null, settingsRepository, pageRepository);
			PageViewModel expectedModel = CreatePageViewModel();
			expectedModel.RawTags = "homepage";
			modelCache.Add(CacheKeys.HomepageKey(), expectedModel, new CacheItemPolicy());

			// Act
			PageViewModel actualModel = pageService.FindHomePage();

			// Assert
			NUnitAssert.That(actualModel.Id, Is.EqualTo(expectedModel.Id));
			NUnitAssert.That(actualModel.VersionNumber, Is.EqualTo(expectedModel.VersionNumber));
			NUnitAssert.That(actualModel.Title, Is.EqualTo(expectedModel.Title));
		}

		public void findhomepage_should_add_to_cache_when_cache_is_empty()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Title = "1", Tags = "homepage" }, "text", "admin", DateTime.UtcNow);

			CacheMock pageCache = new CacheMock();
			PageService pageService = CreatePageService(pageCache, null, settingsRepository, pageRepository);

			// Act
			pageService.FindHomePage();

			// Assert
			NUnitAssert.That(pageCache.CacheItems.Count, Is.EqualTo(1));
			NUnitAssert.That(pageCache.CacheItems.FirstOrDefault().Key, Is.EqualTo(CacheKeys.HomepageKey()));
		}

		public void findbytag_should_load_from_cache()
		{
			string tag1CacheKey = CacheKeys.PagesByTagKey("tag1");
			string tag2CacheKey = CacheKeys.PagesByTagKey("tag2");

			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			CacheMock listCache = new CacheMock();

			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);
			PageViewModel tag1Model = CreatePageViewModel();
			tag1Model.RawTags = "tag1";
			PageViewModel tag2Model = CreatePageViewModel();
			tag2Model.RawTags = "tag2";

			listCache.Add(tag1CacheKey, new List<PageViewModel>() { tag1Model }, new CacheItemPolicy());
			listCache.Add(tag2CacheKey, new List<PageViewModel>() { tag2Model }, new CacheItemPolicy());

			// Act
			IEnumerable<PageViewModel> actualList = pageService.FindByTag("tag1");

			// Assert
			NUnitAssert.That(actualList, Contains.Item(tag1Model));
			NUnitAssert.That(actualList, Is.Not.Contains(tag2Model));
		}

		public void findbytag_should_add_to_cache_when_cache_is_empty()
		{
			// Arrange
			string cacheKey = CacheKeys.PagesByTagKey("tag1");

			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Title = "1", Tags = "tag1" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Title = "2", Tags = "tag2" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Title = "2", Tags = "tag3" }, "text", "admin", DateTime.UtcNow);

			CacheMock listCache = new CacheMock();
			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);

			// Act
			pageService.FindByTag("tag1");

			// Assert
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(1));
			NUnitAssert.That(listCache.CacheItems.FirstOrDefault().Key, Is.EqualTo(cacheKey));
		}

		public void updatepage_should_clear_list_cache_and_pagesummary_cache()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Tags = "homepage" }, "text", "admin", DateTime.UtcNow);
			pageRepository.AddNewPage(new Page() { Tags = "tag2" }, "text", "admin", DateTime.UtcNow);

			CacheMock pageCache = new CacheMock();
			CacheMock listCache = new CacheMock();
			PageService pageService = CreatePageService(pageCache, listCache, settingsRepository, pageRepository);

			PageViewModel homepageModel = CreatePageViewModel();
			homepageModel.Id = 1;
			PageViewModel page2Model = CreatePageViewModel();
			page2Model.Id = 2;

			AddPageCacheItem(pageCache, CacheKeys.HomepageKey(), homepageModel);
			pageCache.Add(CacheKeys.PageViewModelKey(2, 0), page2Model, new CacheItemPolicy());
			AddListCacheItem(listCache, CacheKeys.AllTags(), new List<string>() { "tag1", "tag2" });

			// Act
			pageService.UpdatePage(page2Model);

			// Assert
			NUnitAssert.That(pageCache.CacheItems.Count, Is.EqualTo(1));
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(0));
		}

		public void updatepage_should_remove_homepage_from_cache_when_homepage_is_updated()
		{
			// Arrange
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Tags = "homepage" }, "text", "admin", DateTime.UtcNow);

			CacheMock pageCache = new CacheMock();
			CacheMock listCache = new CacheMock();
			PageService pageService = CreatePageService(pageCache, listCache, settingsRepository, pageRepository);

			PageViewModel homepageModel = CreatePageViewModel();
			homepageModel.RawTags = "homepage";
			pageCache.Add(CacheKeys.HomepageKey(), homepageModel, new CacheItemPolicy());

			// Act
			pageService.UpdatePage(homepageModel);

			// Assert
			NUnitAssert.That(pageCache.CacheItems.Count, Is.EqualTo(0));
		}

		public void renametag_should_clear_listcache()
		{
			// Arrange
			string tag1CacheKey = CacheKeys.PagesByTagKey("tag1");
			SettingsRepositoryMock settingsRepository = new SettingsRepositoryMock();
			PageRepositoryMock pageRepository = new PageRepositoryMock();
			pageRepository.AddNewPage(new Page() { Tags = "homepage, tag1" }, "text1", "admin", DateTime.UtcNow);

			CacheMock listCache = new CacheMock();
			PageViewModel homepageModel = CreatePageViewModel();
			PageViewModel page1Model = CreatePageViewModel();
			AddListCacheItem(listCache, tag1CacheKey, new List<PageViewModel>() { homepageModel, page1Model });

			PageService pageService = CreatePageService(null, listCache, settingsRepository, pageRepository);

			// Act
			pageService.RenameTag("tag1", "some.other.tag"); // calls UpdatePage, which clears the cache

			// Assert
			NUnitAssert.That(listCache.CacheItems.Count, Is.EqualTo(0));
		}

		private PageViewModel CreatePageViewModel(string createdBy = "admin")
		{
			PageViewModel model = new PageViewModel();
			model.Title = "my title";
			model.Id = 1;
			model.CreatedBy = createdBy;
			model.VersionNumber = PageViewModelCache.LATEST_VERSION_NUMBER;

			return model;
		}

		private PageService CreatePageService(ObjectCache<> pageObjectCache, ObjectCache listObjectCache, SettingsRepositoryMock settingsRepository, PageRepositoryMock pageRepository)
		{
			// Stick to memorycache when each one isn't used
			if (pageObjectCache == null)
				pageObjectCache = CacheMock.RoadkillCache;

			if (listObjectCache == null)
				listObjectCache = CacheMock.RoadkillCache;

			// Settings
			var appSettings = new ApplicationSettings() { Installed = true, UseObjectCache = true };
			var userContext = new UserContextStub() { IsLoggedIn = false };
			var builder = new TextMiddlewareBuilder();
			var parser = new MarkdigParser();

			// PageService
			var pageViewModelCache = new PageViewModelCache(appSettings, pageObjectCache);
			var listCache = new ListCache(appSettings, listObjectCache);
			var siteCache = new SiteCache(CacheMock.RoadkillCache);
			var searchService = new SearchServiceMock(appSettings, settingsRepository, pageRepository, builder);

			var historyService = new PageHistoryService(settingsRepository, pageRepository, userContext, pageViewModelCache, builder);
			var pageService = new PageService(appSettings, settingsRepository, pageRepository, searchService, historyService, userContext, listCache, pageViewModelCache, siteCache, builder, parser);

			return pageService;
		}

		private ResultExecutedContext CreateContext(WikiController wikiController)
		{
			// HTTP Context
			ControllerContext controllerContext = new Mock<ControllerContext>().Object;
			MvcMockContainer container = new MvcMockContainer();
			HttpContextBase context = MvcMockHelpers.FakeHttpContext(container);
			controllerContext.HttpContext = context;

			// ResultExecutedContext
			ActionResult result = new ViewResult();
			Exception exception = new Exception();
			bool cancelled = true;

			ResultExecutedContext filterContext = new ResultExecutedContext(controllerContext, result, cancelled, exception);
			filterContext.Controller = wikiController;
			filterContext.RouteData.Values.Add("id", 1);
			filterContext.HttpContext = context;

			return filterContext;
		}

		private void AddPageCacheItem(CacheMock cache, string key, object value)
		{
			cache.Add(CacheKeys.PAGEVIEWMODEL_CACHE_PREFIX + key, value, new CacheItemPolicy());
		}

		private void AddListCacheItem(CacheMock cache, string key, object value)
		{
			cache.Add(CacheKeys.ListCacheKey(key), value, new CacheItemPolicy());
		}
	}
}