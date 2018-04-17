using Roadkill.Core;
using Roadkill.Core.Cache;
using Roadkill.Core.Configuration;
using Roadkill.Core.Security;
using Roadkill.Core.Services;
using Roadkill.Tests.Unit.StubsAndMocks;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using Roadkill.Core.Database;
using Roadkill.Core.DependencyResolution.StructureMap;
using StructureMap;
using Roadkill.Core.Text;
using Roadkill.Core.Text.Parsers.Markdig;
using Roadkill.Core.Text.TextMiddleware;

namespace Roadkill.Tests.Unit
{
	public class MocksAndStubsContainer
	{
		public ApplicationSettings ApplicationSettings { get; set; }
		public ConfigReaderWriterStub ConfigReaderWriter { get; set; }
		public IUserContext UserContext { get; set; }

		public MemoryCache MemoryCache { get; set; }
		public ListCache ListCache { get; set; }
		public SiteCache SiteCache { get; set; }
		public PageViewModelCache PageViewModelCache { get; set; }
		
		public PluginFactoryMock PluginFactory { get; set; }
        public EmailClientMock EmailClient { get; set; }

		public UserServiceMock UserService { get; set; }
		public PageService PageService { get; set; }
		public SearchServiceMock SearchService { get; set; }
		public PageHistoryService HistoryService { get; set; }
		public SettingsService SettingsService { get; set; }
		public IFileService FileService { get; set; }

		public RepositoryFactoryMock RepositoryFactory { get; set; }
		public SettingsRepositoryMock SettingsRepository { get; set; }
		public UserRepositoryMock UserRepository { get; set; }
		public PageRepositoryMock PageRepository { get; set; }
		public InstallerRepositoryMock InstallerRepository { get; set; }
		public DatabaseTesterMock DatabaseTester { get;set; }
		public Container StructureMapContainer { get; set; }
		public StructureMapServiceLocator Locator { get; set; }
		public InstallationService InstallationService { get; set; }

		/// <summary>
		/// Creates a new instance of MocksAndStubsContainer.
		/// </summary>
		/// <param name="useCacheMock">The 'Roadkill' MemoryCache is used by default, but as this is static it can have problems with 
		/// the test runner unless you clear the Container.MemoryCache on setup each time, but then doing that doesn't give a realistic 
		/// reflection of how the MemoryCache is used inside an ASP.NET environment.</param>
		public MocksAndStubsContainer(bool useCacheMock = false)
		{
			ApplicationSettings = new ApplicationSettings();
			ApplicationSettings.Installed = true;
			ApplicationSettings.AttachmentsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments");
			ConfigReaderWriter = new ConfigReaderWriterStub();

			// Cache
			MemoryCache = useCacheMock ? new CacheMock() : CacheMock.RoadkillCache;
			ListCache = new ListCache(ApplicationSettings, MemoryCache);
			SiteCache = new SiteCache(MemoryCache);
			PageViewModelCache = new PageViewModelCache(ApplicationSettings, MemoryCache);

			// Repositories
			SettingsRepository = new SettingsRepositoryMock();
			SettingsRepository.SiteSettings = new SiteSettings();
			UserRepository = new UserRepositoryMock();
			PageRepository = new PageRepositoryMock();
			InstallerRepository = new InstallerRepositoryMock();

			RepositoryFactory = new RepositoryFactoryMock()
			{
				SettingsRepository = SettingsRepository,
				UserRepository = UserRepository,
				PageRepository = PageRepository,
				InstallerRepository = InstallerRepository
			};
			DatabaseTester = new DatabaseTesterMock();

            // Text middleware
		    TextMiddlewareBuilder = new TextMiddlewareBuilder();
		    MarkupParser = new MarkdigParser();

            PluginFactory = new PluginFactoryMock();

            // Services
            // Dependencies for PageService. Be careful to make sure the class using this Container isn't testing the mock.
            SettingsService = new SettingsService(RepositoryFactory, ApplicationSettings);
			UserService = new UserServiceMock(ApplicationSettings, UserRepository);
			UserContext = new UserContext(UserService);

            SearchService = new SearchServiceMock(ApplicationSettings, SettingsRepository, PageRepository, TextMiddlewareBuilder);
			SearchService.PageContents = PageRepository.PageContents;
			SearchService.Pages = PageRepository.Pages;

			HistoryService = new PageHistoryService(SettingsRepository, PageRepository, UserContext,
				PageViewModelCache, TextMiddlewareBuilder);

			FileService = new FileServiceMock();
			PageService = new PageService(ApplicationSettings, SettingsRepository, PageRepository, SearchService, HistoryService,
				UserContext, ListCache, PageViewModelCache, SiteCache, TextMiddlewareBuilder, MarkupParser);

			StructureMapContainer = new Container(x =>
			{
				x.AddRegistry(new TestsRegistry(this));
			});

			Locator = new StructureMapServiceLocator(StructureMapContainer, false);

			InstallationService = new InstallationService((databaseName, connectionString) =>
			{
				InstallerRepository.DatabaseName = databaseName;
				InstallerRepository.ConnectionString = connectionString;

				return InstallerRepository;
			}, Locator);

			// EmailTemplates
			EmailClient = new EmailClientMock();
		}

	    public MarkdigParser MarkupParser { get; set; }

	    public TextMiddlewareBuilder TextMiddlewareBuilder { get; set; }

	    public void ClearCache()
		{
			foreach (string key in MemoryCache.Select(x => x.Key))
				MemoryCache.Remove(key);
		}

		public class TestsRegistry : Registry
		{
			public TestsRegistry(MocksAndStubsContainer mockContainer)
			{
				For<IFileService>().Use(mockContainer.FileService);
				For<IRepositoryFactory>().Use(mockContainer.RepositoryFactory);
			}
		}
	}
}
