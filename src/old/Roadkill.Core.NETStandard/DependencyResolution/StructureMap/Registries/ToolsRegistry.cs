using Roadkill.Core.Domain.Export;
using Roadkill.Core.Email;
using Roadkill.Core.Import;
using StructureMap;
using StructureMap.Graph;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
    public class ToolsRegistry : Registry
	{
		public ToolsRegistry()
		{
			Scan(ScanTypes);
			ConfigureInstances();
        }

		private void ScanTypes(IAssemblyScanner scanner)
		{
            scanner.TheCallingAssembly();
            scanner.SingleImplementationsOfInterface();
            scanner.WithDefaultConventions();

            // Emails
            scanner.AddAllTypesOf<SignupEmail>();
			scanner.AddAllTypesOf<ResetPasswordEmail>();

			// Export
			scanner.AddAllTypesOf<WikiExporter>();
		}

		private void ConfigureInstances()
		{
			// Screwturn importer
			For<IWikiImporter>().Use<ScrewTurnImporter>();

			// Emails
			For<SignupEmail>().Use<SignupEmail>();
			For<ResetPasswordEmail>().Use<ResetPasswordEmail>();
		}
	}
}