using System;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Roadkill.Core.Database;
using Roadkill.Core.Plugins;
using Roadkill.Core.Text.CustomTokens;
using Roadkill.Core.Text.Parsers;
using Roadkill.Core.Text.Parsers.Markdig;
using Roadkill.Core.Text.Sanitizer;
using Roadkill.Core.Text.TextMiddleware;
using Roadkill.Tests.Unit.StubsAndMocks;
using Roadkill.Tests.Unit.StubsAndMocks.Mvc;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
	[TestFixture]
	[Category("Unit")]
	public class TextRegistryTests : RegistryTestsBase
	{
		private RepositoryFactoryMock _repositoryFactory;
		private UrlHelperMock _urlHelper;

		[SetUp]
		public void Setup()
		{
			_urlHelper = new UrlHelperMock();

			// Inject a fake HttpContext for UrlHelper, used by UrlResolver
			var httpContext = MvcMockHelpers.FakeHttpContext("~/url");
			Container.Configure(x => x.For<HttpContextBase>().Use(httpContext));

			// Inject a fake SettingsRepository for TextPlugins
			Container.Configure(x =>
			{
				_repositoryFactory = new RepositoryFactoryMock();
				_repositoryFactory.SettingsRepository = new SettingsRepositoryMock();

				x.For<IRepositoryFactory>().Use(_repositoryFactory);

				// And UrlHelper so no RouteTable is required
				x.For<UrlHelper>().Use(_urlHelper);
			});
		}

		private void AddPage(string title)
		{
			_repositoryFactory.PageRepository.AddNewPage(new Page() { Id = 1, Title = title }, "some text", "user", DateTime.Today);
		}

		[Test]
		public void should_register_types_with_instances()
		{
			// Arrange + Act + Assert
			AssertDefaultType<IPluginFactory, PluginFactory>();
			AssertDefaultType<IMarkupParser, MarkdigParser>();
			AssertDefaultType<IHtmlSanitizerFactory, HtmlSanitizerFactory>();
			AssertDefaultType<CustomTokenParser, CustomTokenParser>();
		}

		[Test]
		public void should_construct_TextMiddlewareBuilder_and_parse_basic_markup()
		{
			// Arrange
			IContainer container = Container;

			// Act
			var builder = container.GetInstance<TextMiddlewareBuilder>();

			// Assert
			Assert.That(builder, Is.Not.Null);

			string html = builder.Execute("**markdown**");
			Assert.That(html, Is.EqualTo("<p><strong>markdown</strong></p>\n")); // a basic smoke test of the middleware chain
		}

		[Test]
		public void should_register_TextMiddlewareBuilder_in_the_correct_order()
		{
			// Arrange
			IContainer container = Container;

			// Act
			var builder = container.GetInstance<TextMiddlewareBuilder>();

			// Assert
			Assert.That(builder, Is.Not.Null);
			Assert.That(builder.MiddlewareItems[0], Is.TypeOf<TextPluginBeforeParseMiddleware>());
			Assert.That(builder.MiddlewareItems[1], Is.TypeOf<MarkupParserMiddleware>());
			Assert.That(builder.MiddlewareItems[2], Is.TypeOf<HarmfulTagMiddleware>());
			Assert.That(builder.MiddlewareItems[3], Is.TypeOf<CustomTokenMiddleware>());
			Assert.That(builder.MiddlewareItems[4], Is.TypeOf<TextPluginAfterParseMiddleware>());
		}

		[Test]
		[TestCase("http://i223.photobucket.com/albums/dd45/wally2603/91e7840f.jpg")]
		[TestCase("https://i223.photobucket.com/albums/dd45/wally2603/91e7840f.jpg")]
		public void TextMiddlewareBuilder_should_Not_Rewrite_Images_As_Internal_That_Start_With_Known_Prefixes(string imageUrl)
		{
			// Arrange
			IContainer container = Container;

			// Act
			var builder = container.GetInstance<TextMiddlewareBuilder>();

			// Assert
			Assert.That(builder, Is.Not.Null);

			string html = builder.Execute("![Image title](" + imageUrl + ")");
			// assert image was called/html
			Assert.That(html, Is.EqualTo("<p><img src=\"" + imageUrl + "\" class=\"img-responsive\"></p>\n"));
		}

		[Test]
		public void TextMiddlewareBuilder_should_remove_script_link_iframe_frameset_frame_applet_tags_from_text()
		{
			// Arrange
			string markdown = " some text <script type=\"text/html\">while(true)alert('lolz');</script>" +
				"<iframe src=\"google.com\"></iframe><frame>blah</frame> <applet code=\"MyApplet.class\" width=100 height=140></applet>" +
				"<frameset src='new.html'></frameset>";
			string expectedHtml = "<p>some text blah </p>\n";

			IContainer container = Container;
			var builder = container.GetInstance<TextMiddlewareBuilder>();

			// Act
			string actualHtml = builder.Execute(markdown);

			// Assert
			Assert.That(actualHtml, Is.EqualTo(expectedHtml));
		}

		[Test]
		public void should_use_markdigparser_as_imarkupparser_using_factory()
		{
			// Arrange
			IContainer container = Container;

			// Act
			var mardigParserFactory = container.GetInstance<IMarkdigParserFactory>();
			var markupParser = container.GetInstance<IMarkupParser>();

			// Assert
			Assert.That(mardigParserFactory, Is.Not.Null);
			Assert.That(mardigParserFactory, Is.TypeOf<MarkdigParserFactory>());

			Assert.That(markupParser, Is.Not.Null);
			Assert.That(markupParser, Is.TypeOf<MarkdigParser>());
		}
	}
}