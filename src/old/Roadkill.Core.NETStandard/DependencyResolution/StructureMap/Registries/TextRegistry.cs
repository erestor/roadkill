using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Plugins;
using Roadkill.Core.Text.CustomTokens;
using Roadkill.Core.Text.Parsers.Markdig;
using Roadkill.Core.Text.Plugins;
using Roadkill.Core.Text.Sanitizer;
using Roadkill.Core.Text.TextMiddleware;
using StructureMap;
using StructureMap.Graph;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
	public class TextRegistry : Registry
	{
		public TextRegistry()
		{
			Scan(ScanTypes);
			ConfigureInstances();
		}

		private void ScanTypes(IAssemblyScanner scanner)
		{
			scanner.TheCallingAssembly();
			scanner.SingleImplementationsOfInterface();
			scanner.WithDefaultConventions();

			scanner.AddAllTypesOf<CustomTokenParser>();
		}

		private void ConfigureInstances()
		{
			For<IPluginFactory>().Use<PluginFactory>();
			For<IMarkdigParserFactory>().Use<MarkdigParserFactory>();
			For<IHtmlSanitizerFactory>().Use<HtmlSanitizerFactory>();

			For<TextMiddlewareBuilder>()
				.AlwaysUnique()
				.Use("TextMiddlewareBuilder", ctx =>
				{
					var builder = new TextMiddlewareBuilder();

					var textPluginRunner = ctx.GetInstance<TextPluginRunner>();
					var markupParser = GetMarkdigParser(ctx);
					var htmlSanitizerFactory = ctx.GetInstance<IHtmlSanitizerFactory>();
					var customTokenParser = ctx.GetInstance<CustomTokenParser>();

					builder.Use(new TextPluginBeforeParseMiddleware(textPluginRunner));
					builder.Use(new MarkupParserMiddleware(markupParser));
					builder.Use(new HarmfulTagMiddleware(htmlSanitizerFactory));
					builder.Use(new CustomTokenMiddleware(customTokenParser));
					builder.Use(new TextPluginAfterParseMiddleware(textPluginRunner));

					return builder;
				}).Singleton();
		}

		private MarkdigParser GetMarkdigParser(IContext ctx)
		{
			// TODO: NETStandard - change the factory to take an IUrlHelper

			var pageRepository = ctx.GetInstance<IPageRepository>();
			var applicationSettings = ctx.GetInstance<ApplicationSettings>();
			var urlHelper = ctx.GetInstance<UrlHelper>();
			var factory = ctx.GetInstance<IMarkdigParserFactory>();

			return factory.Create(pageRepository, applicationSettings, urlHelper);
		}
	}
}