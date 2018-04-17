using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.DependencyResolution;
using Roadkill.Core.DependencyResolution.StructureMap;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using Roadkill.Tests.Unit.StubsAndMocks;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
    public class RegistryTestsBase
    {
        protected IContainer Container;
	    protected ConfigReaderWriterStub ConfigReaderWriterStub;

		[SetUp]
	    public void Setup()
	    {
			// Create a basic ConfigReaderWriter with some settings
			ConfigReaderWriterStub = new ConfigReaderWriterStub();
			ConfigReaderWriterStub.ApplicationSettings.DatabaseName = "SqlServer2008";
			ConfigReaderWriterStub.ApplicationSettings.ConnectionString = "none empty connection string";
			ConfigReaderWriterStub.ApplicationSettings.UseHtmlWhiteList = true;

			Container = CreateContainerWithRoadkillRegistry();
		}

		private IContainer CreateContainerWithRoadkillRegistry()
        {
			// Need a RoadkillRegistry for all the dependencies other registries require
			var container = new Container(c =>
            {
                c.AddRegistry(new RoadkillRegistry(ConfigReaderWriterStub));
            });

			// Some places that require bastard injection reference the LocatorStartup.Locator
			LocatorStartup.Locator = new StructureMapServiceLocator(container, false);

            return container;
        }

	    public void InjectApplicationSettings(ApplicationSettings applicationSettings)
	    {
		    var configReaderWriter = new ConfigReaderWriterStub() {ApplicationSettings = applicationSettings};
		    Container.Configure(x => x.For<ConfigReaderWriter>().Use(configReaderWriter));
	    }

	    public void AssertDefaultType<TParent, TConcrete>(IContainer container = null)
        {
            // Arrange
            if (container == null)
                container = Container;

            // Act
            TParent instance = container.GetInstance<TParent>();

            // Assert
            Assert.That(instance, Is.TypeOf<TConcrete>());
        }
    }
}