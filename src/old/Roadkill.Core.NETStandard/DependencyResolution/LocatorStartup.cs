using Roadkill.Core.Configuration;
using Roadkill.Core.DependencyResolution.StructureMap;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using StructureMap;

// TODO: NETStandard - make this into Middleware
//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LocatorStartup), "StartMVC")]
//[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(LocatorStartup), "AfterInitialization")]
//[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(LocatorStartup), "End")]

namespace Roadkill.Core.DependencyResolution
{
	// This class does the following:
	// - Is called on app startup
	// - Creates a new Container that uses RoadkillRegistry, which does the scanning + instance mapping.
	// - Uses a new StructureMapServiceLocator as the MVC/WebApi service locator
	// - Uses a StructureMapScopeModule HttpModule to create a new container per request
	// - Does additional MVC/WebApi plumbing after application start in AfterInitialization

	public static class LocatorStartup
	{
		public static IContainer Container { get; set; }

		public static void StartMVC()
		{
			// TODO: NETStandard - change the config loader class
			StartMVCInternal(new RoadkillRegistry(new FullTrustConfigReaderWriter()), true);
		}

		internal static void StartMVCInternal(RoadkillRegistry registry, bool isWeb)
		{
			IContainer container = new Container(c =>
			{
				c.AddRegistry(registry);
				c.AddRegistry<TextRegistry>();
			});

			Container = container;

			// TODO: NETStandard - make this use the new MVC DI
			// MVC locator
			//DependencyResolver.SetResolver(Container);
			//DynamicModuleUtility.RegisterModule(typeof(StructureMapHttpModule));
		}

		// Must be run **after** the app has started/initialized via WebActivor
		public static void AfterInitialization()
		{
			// TODO: NETStandard - make this use the new MVC DI
			// Setup the additional MVC DI stuff
			//var settings = Container.GetInstance<ApplicationSettings>();
			//AfterInitializationInternal(Container.Container, settings);

			//Log.ConfigureLogging(settings);
		}

		internal static void AfterInitializationInternal(IContainer container, ApplicationSettings appSettings)
		{
			// TODO: NETStandard - make this use the new MVC DI
			// WebApi: service locator
			//GlobalConfiguration.Configuration.DependencyResolver = Container;

			//// WebAPI: attributes
			//var webApiProvider = new MvcAttributeProvider(GlobalConfiguration.Configuration.Services.GetFilterProviders(), container);
			//GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.Filters.IFilterProvider), webApiProvider);

			//// MVC: attributes
			//var mvcProvider = new MvcAttributeProvider(container);
			//FilterProviders.Providers.Add(mvcProvider); // attributes

			//// MVC: Models with ModelBinding that require DI
			//ModelBinders.Binders.Add(typeof(UserViewModel), new UserViewModelModelBinder());
			//ModelBinders.Binders.Add(typeof(SettingsViewModel), new SettingsViewModelBinder());
		}

		public static void End()
		{
			Container.Dispose();
		}
	}
}