using NUnit.Framework;
using Roadkill.Core.Domain.Export;
using Roadkill.Core.Email;
using Roadkill.Core.Import;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
    [TestFixture]
	[Category("Unit")]
	public class ToolsRegistryTests : RegistryTestsBase
    {
		[Test]
		public void should_register_email_types()
		{
			// Arrange + Act + Assert
			AssertDefaultType<SignupEmail, SignupEmail>();
			AssertDefaultType<ResetPasswordEmail, ResetPasswordEmail>();
		}

		[Test]
		public void should_register_default_importers_and_exporter()
		{
			// Arrange
			IContainer container = Container;

			// Act
			IWikiImporter wikiImporter = container.GetInstance<IWikiImporter>();
			WikiExporter wikiExporter = container.GetInstance<WikiExporter>();

			// Assert
			Assert.That(wikiImporter, Is.TypeOf<ScrewTurnImporter>());
			Assert.That(wikiExporter, Is.TypeOf<WikiExporter>());
		}
	}
}
