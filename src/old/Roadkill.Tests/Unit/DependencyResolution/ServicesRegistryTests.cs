using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using Roadkill.Core.Security;
using Roadkill.Core.Security.Windows;
using Roadkill.Core.Services;
using Roadkill.Tests.Unit.StubsAndMocks;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
    [TestFixture]
	[Category("Unit")]
	public class ServicesRegistryTests : RegistryTestsBase
    {
		[Test]
		public void should_register_services()
		{
			// Arrange
			var settings = new ApplicationSettings();
			settings.ConnectionString = "none empty connection string";
			settings.LdapConnectionString = "LDAP://dc=roadkill.org"; // for ActiveDirectoryUserService
			settings.AdminRoleName = "admins";
			settings.EditorRoleName = "editors";
			InjectApplicationSettings(settings);

			var container = Container;

			// Act +  Assert
			Assert.That(container.GetInstance<SearchService>(), Is.Not.Null);
			Assert.That(container.GetInstance<PageHistoryService>(), Is.Not.Null);
			Assert.That(container.GetInstance<PageService>(), Is.Not.Null);
			Assert.That(container.GetInstance<FormsAuthUserService>(), Is.Not.Null);
			Assert.That(container.GetInstance<ActiveDirectoryUserService>(), Is.Not.Null);
		}

		[Test]
		public void should_use_localfileservice_by_default()
		{
			// Arrange + Act + Assert
			AssertDefaultType<IFileService, LocalFileService>();
		}

		[Test]
		public void should_use_azurefileservice_when_setting_has_azure_true()
		{
			// Arrange
			ApplicationSettings settings = new ApplicationSettings();
			settings.ConnectionString = "none empty connection string";
			settings.UseAzureFileStorage = true;
			InjectApplicationSettings(settings);

			var container = Container;

			// Act + Assert
			Assert.That(container.GetInstance<IFileService>(), Is.TypeOf(typeof(AzureFileService)));
		}
	}
}
