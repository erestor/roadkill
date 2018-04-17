using System;
using System.IO;
using NUnit.Framework;
using Roadkill.Core;
using Roadkill.Core.Configuration;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using Roadkill.Core.Security;
using Roadkill.Core.Security.Windows;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
    [TestFixture]
	[Category("Unit")]
	public class SecurityRegistryTests : RegistryTestsBase
    {
		[Test]
		public void should_use_usercontext_by_default()
		{
			// Arrange + Act + Assert
			AssertDefaultType<IUserContext, UserContext>();
		}

		[Test]
		public void should_use_formsauthuserservice_by_default()
		{
			// Arrange + Act + Assert
			AssertDefaultType<UserServiceBase, FormsAuthUserService>();
		}

		[Test]
		public void should_load_custom_userservice_using_short_type_format()
		{
			// Arrange
			ApplicationSettings settings = new ApplicationSettings();
			settings.ConnectionString = "none empty connection string";
			settings.UserServiceType = "Roadkill.Plugins.TestUserService, Roadkill.Plugins";
			settings.PluginsBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
			InjectApplicationSettings(settings);
			Console.WriteLine(settings.UserServiceType);

			var container = Container;

			// Act
			UserServiceBase userService = container.GetInstance<UserServiceBase>();

			// Act
			Assert.That(userService, Is.Not.Null);
			Assert.That(userService.GetType().AssemblyQualifiedName, Is.StringContaining(settings.UserServiceType));
		}

		[Test]
		public void should_load_custom_userservice_using_assemblyqualifiedname()
		{
			// Arrange
			ApplicationSettings settings = new ApplicationSettings();
			settings.ConnectionString = "none empty connection string";
			settings.UserServiceType = typeof(Roadkill.Plugins.TestUserService).AssemblyQualifiedName;
			settings.PluginsBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
			InjectApplicationSettings(settings);
			Console.WriteLine(settings.UserServiceType);

			var container = Container;

			// Act
			UserServiceBase userService = container.GetInstance<UserServiceBase>();

			// Act
			Assert.That(userService, Is.Not.Null);
			Assert.That(userService.GetType().AssemblyQualifiedName, Is.EqualTo(settings.UserServiceType));
		}

		[Test]
		public void should_load_activedirectory_userservice_when_usewindowsauth_is_true()
		{
			// Arrange
			ApplicationSettings settings = new ApplicationSettings();
			settings.ConnectionString = "none empty connection string";
			settings.UseWindowsAuthentication = true;
			settings.LdapConnectionString = "LDAP://dc=roadkill.org";
			settings.AdminRoleName = "admins";
			settings.EditorRoleName = "editors";
			InjectApplicationSettings(settings);

			var container = Container;

			// Act + Assert
			Assert.That(container.GetInstance<UserServiceBase>(), Is.TypeOf(typeof(ActiveDirectoryUserService)));
		}

		[Test]
		public void should_register_default_security_providers()
		{
			// Arrange
			IContainer container = Container;

			// Act
			IAuthorizationProvider authProvider = container.GetInstance<IAuthorizationProvider>();

			// Assert
			Assert.That(authProvider, Is.TypeOf<AuthorizationProvider>());
			IActiveDirectoryProvider adProvider = container.GetInstance<IActiveDirectoryProvider>();
			Assert.That(adProvider, Is.TypeOf<ActiveDirectoryProvider>());
		}
	}
}
