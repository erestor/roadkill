using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Database.Repositories;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Web;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
	public class RepositoryRegistry : Registry
	{
		public RepositoryRegistry()
		{
			Scan(ScanTypes);
			ConfigureInstances();
		}

		private void ScanTypes(IAssemblyScanner scanner)
		{
            scanner.TheCallingAssembly();
            scanner.SingleImplementationsOfInterface();
            scanner.WithDefaultConventions();

            scanner.AddAllTypesOf<ISettingsRepository>();
			scanner.AddAllTypesOf<IUserRepository>();
			scanner.AddAllTypesOf<IPageRepository>();
		}

		private void ConfigureInstances()
		{
			// TODO: All services should take an IRepositoryFactory, no injection should be needed for IXYZRepository
			For<IRepositoryFactory>()
				.Singleton()
				.Use<RepositoryFactory>();

			For<ISettingsRepository>()
				.HybridHttpOrThreadLocalScoped()
				.Use("ISettingsRepository", x =>
				{
					ApplicationSettings appSettings = x.GetInstance<ApplicationSettings>();
					return x.TryGetInstance<IRepositoryFactory>()
						.GetSettingsRepository(appSettings.DatabaseName, appSettings.ConnectionString);
				});

			For<IUserRepository>()
				.HybridHttpOrThreadLocalScoped()
				.Use("IUserRepository", x =>
				{
					ApplicationSettings appSettings = x.GetInstance<ApplicationSettings>();
					return x.TryGetInstance<IRepositoryFactory>()
						.GetUserRepository(appSettings.DatabaseName, appSettings.ConnectionString);
				});

			For<IPageRepository>()
				.HybridHttpOrThreadLocalScoped()
				.Use("IPageRepository", x =>
				{
					ApplicationSettings appSettings = x.GetInstance<ApplicationSettings>();
					return x.TryGetInstance<IRepositoryFactory>()
						.GetPageRepository(appSettings.DatabaseName, appSettings.ConnectionString);
				});
		}
	}
}