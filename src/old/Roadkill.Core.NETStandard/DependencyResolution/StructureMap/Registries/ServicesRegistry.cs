using Roadkill.Core.Configuration;
using Roadkill.Core.Services;
using StructureMap;
using StructureMap.Graph;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
	public class ServicesRegistry : Registry
	{
		public ServicesRegistry()
		{
			Scan(ScanTypes);
			ConfigureInstances();
		}

		private void ScanTypes(IAssemblyScanner scanner)
		{
			scanner.TheCallingAssembly();
			scanner.SingleImplementationsOfInterface();
			scanner.WithDefaultConventions();

			scanner.AddAllTypesOf<IPageService>();
			scanner.AddAllTypesOf<ISearchService>();
			scanner.AddAllTypesOf<ISettingsService>();
			scanner.AddAllTypesOf<IFileService>();
			scanner.AddAllTypesOf<IInstallationService>();
		}

		private void ConfigureInstances()
		{
			// Services
			// TODO: NETStandard - use Structuremap project
			//For<IPageService>().HybridHttpOrThreadLocalScoped().Use<PageService>();
			//For<IInstallationService>().HybridHttpOrThreadLocalScoped().Use<InstallationService>();

			// File service
			For<IFileService>()
				//.HybridHttpOrThreadLocalScoped()
				.Use<IFileService>("IFileService", ctx =>
				{
					var appSettings = ctx.GetInstance<ApplicationSettings>();
					if (appSettings.UseAzureFileStorage) // remove this property
					{
						//return ctx.GetInstance<AzureFileService>();
					}

					return ctx.GetInstance<LocalFileService>();
				});
		}
	}
}