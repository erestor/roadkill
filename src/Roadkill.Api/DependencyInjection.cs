using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Roadkill.Api
{
	public class DependencyInjection
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.Scan(scan => scan
			   .FromAssemblyOf<Roadkill.Api.DependencyInjection>()

			   // SomeClass => ISomeClass
			   .AddClasses()
			   .UsingRegistrationStrategy(RegistrationStrategy.Skip)
			   .AsMatchingInterface()
			   .WithTransientLifetime()
		   );
		}
	}
}