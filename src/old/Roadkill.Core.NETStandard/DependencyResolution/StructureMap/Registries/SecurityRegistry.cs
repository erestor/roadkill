using System;
using Roadkill.Core.Configuration;
using Roadkill.Core.Security;
using Roadkill.Core.Security.Windows;
using StructureMap;
using StructureMap.Building;
using StructureMap.Graph;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
	public class SecurityRegistry : Registry
	{
		public SecurityRegistry()
		{
			Scan(ScanTypes);
			ConfigureInstances();
		}

		private void ScanTypes(IAssemblyScanner scanner)
		{
			scanner.TheCallingAssembly();
			scanner.SingleImplementationsOfInterface();
			scanner.WithDefaultConventions();

			scanner.With(new AbstractClassConvention<UserServiceBase>());
			scanner.AddAllTypesOf<IActiveDirectoryProvider>();
			scanner.AddAllTypesOf<IUserContext>();
		}

		private void ConfigureInstances()
		{
			// TODO: NETStandard - use Structuremap project
			For<IAuthorizationProvider>().Use<AuthorizationProvider>();
			//For<IUserContext>().HybridHttpOrThreadLocalScoped();
			For<IActiveDirectoryProvider>().Use<ActiveDirectoryProvider>();

			// User service
			For<UserServiceBase>()
				//.HybridHttpOrThreadLocalScoped()
				.Use("UserServiceBase", ctx =>
			{
				var appsettings = ctx.GetInstance<ApplicationSettings>();
				string userServiceTypeName = appsettings.UserServiceType;

				if (appsettings.UseWindowsAuthentication)
				{
					return ctx.GetInstance<ActiveDirectoryUserService>();
				}
				else if (!string.IsNullOrEmpty(userServiceTypeName))
				{
					try
					{
						Type userServiceType = Type.GetType(userServiceTypeName, false, false);
						if (userServiceType == null)
							throw new IoCException(null, "Unable to find UserService type {0}. Make sure you use the AssemblyQualifiedName.", userServiceTypeName);

						return (UserServiceBase)ctx.GetInstance(userServiceType);
					}
					catch (StructureMapBuildException)
					{
						throw new IoCException(null, "Unable to find UserService type {0}", userServiceTypeName);
					}
				}

				return ctx.GetInstance<FormsAuthUserService>();
			});
		}
	}
}