using System.IO;
using Roadkill.Core.Cache;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database.Repositories;
using Roadkill.Core.Plugins;
using StructureMap;
using StructureMap.Graph;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
    public class PluginsRegistry : Registry
	{
		public ApplicationSettings ApplicationSettings { get; set; }

		public PluginsRegistry(ConfigReaderWriter configReader)
		{
			ApplicationSettings = configReader.GetApplicationSettings();

			Scan(ScanTypes);
			ConfigureInstances(configReader);

		    IncludeRegistry<RepositoryRegistry>();
		    IncludeRegistry<MvcRegistry>();
		}

		private static void CopyPlugins(ApplicationSettings applicationSettings)
		{
			string pluginsDestPath = applicationSettings.PluginsBinPath;
			if (!Directory.Exists(pluginsDestPath))
				Directory.CreateDirectory(pluginsDestPath);

			PluginFileManager.CopyPlugins(applicationSettings);
		}

		private void ScanTypes(IAssemblyScanner scanner)
		{
			// Scan plugins: this includes everything e.g repositories, UserService, FileService TextPlugins
			CopyPlugins(ApplicationSettings);
			foreach (string subDirectory in Directory.GetDirectories(ApplicationSettings.PluginsBinPath))
			{
				scanner.AssembliesFromPath(subDirectory);
			}

            scanner.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("Roadkill.Plugins"));
            scanner.With(new AbstractClassConvention<TextPlugin>());
		    scanner.With(new AbstractClassConvention<SpecialPagePlugin>());
		}

		private void ConfigureInstances(ConfigReaderWriter configReader)
		{
			For<IPluginFactory>().Singleton().Use<PluginFactory>();
			
			// Setter inject the *internal* properties for the text plugins
			For<TextPlugin>().OnCreationForAll("set plugin cache", (ctx, plugin) =>
			{
			    plugin.PluginCache = ctx.GetInstance<IPluginCache>();
			});
			For<TextPlugin>().OnCreationForAll("set plugin settings repository", (ctx, plugin) =>
			{
			    plugin.SettingsRepository = ctx.GetInstance<ISettingsRepository>();
			});
		}
	}
}