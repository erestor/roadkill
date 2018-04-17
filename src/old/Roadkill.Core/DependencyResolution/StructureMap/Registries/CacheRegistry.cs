using System.Runtime.Caching;
using Roadkill.Core.Cache;
using StructureMap;
using StructureMap.Graph;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
    public class CacheRegistry : Registry
	{
		public CacheRegistry()
		{
			Scan(ScanTypes);
			ConfigureInstances();
        }

		private void ScanTypes(IAssemblyScanner scanner)
		{
            scanner.TheCallingAssembly();
            scanner.SingleImplementationsOfInterface();
            scanner.WithDefaultConventions();

            scanner.AddAllTypesOf<ListCache>();
			scanner.AddAllTypesOf<PageViewModelCache>();
		}

		private void ConfigureInstances()
		{
			For<ObjectCache>().Singleton().Use(new MemoryCache("Roadkill"));
			For<ListCache>().Singleton();
			For<SiteCache>().Singleton();
			For<PageViewModelCache>().Singleton();
			For<IPluginCache>().Use<SiteCache>();
		}
	}
}