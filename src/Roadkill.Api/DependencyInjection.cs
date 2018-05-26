using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Roadkill.Text;
using Roadkill.Text.Sanitizer;
using Roadkill.Text.TextMiddleware;
using Scrutor;

namespace Roadkill.Api
{
	public class DependencyInjection
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging();

			// Markdown
			services.AddScoped<TextSettings>();
			services.AddScoped<IHtmlWhiteListProvider, HtmlWhiteListProvider>();
			services.AddScoped<ITextMiddlewareBuilder>(provider =>
			{
				var textSettings = provider.GetService<TextSettings>();
				var logger = provider.GetService<ILogger>();

				return TextMiddlewareBuilder.Default(textSettings, logger);
			});

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