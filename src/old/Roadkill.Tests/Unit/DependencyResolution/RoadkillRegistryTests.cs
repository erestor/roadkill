using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using Roadkill.Tests.Unit.StubsAndMocks;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
	[TestFixture]
	[Category("Unit")]
	public class RoadkillRegistryTests
	{
		private IContainer CreateContainer()
		{
			var configReaderWriterStub = new ConfigReaderWriterStub();
			configReaderWriterStub.ApplicationSettings.ConnectionString = "none empty connection string";

			var roadkillRegistry = new RoadkillRegistry(configReaderWriterStub);
			var container = new Container(c =>
			{
				c.AddRegistry(roadkillRegistry);
			});

			return container;
		}

		[Test]
		public void should_get_application_settings_from_config_reader_instance()
		{
			// Arrange
			IContainer container = CreateContainer();

			// Act
			ConfigReaderWriter configReader = container.GetInstance<ConfigReaderWriter>();
			ApplicationSettings settings = container.GetInstance<ApplicationSettings>();

			// Assert
			Assert.That(settings, Is.Not.Null);
			Assert.That(configReader, Is.TypeOf<ConfigReaderWriterStub>());
		}
	}
}
