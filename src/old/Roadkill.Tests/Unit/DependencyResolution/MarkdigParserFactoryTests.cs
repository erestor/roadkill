using System;
using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Text.Parsers.Images;
using Roadkill.Core.Text.Parsers.Links;
using Roadkill.Core.Text.Parsers.Markdig;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.DependencyResolution
{
	public class MarkdigParserFactoryTests
	{
		private UrlHelperMock _urlHelper;
		private PageRepositoryMock _pageRepository;
		private ApplicationSettings _applicationSettings;
		private MarkdigParserFactory _factory;
		private MarkdigParser _markupParser;

		[SetUp]
		public void Setup()
		{
			_urlHelper = new UrlHelperMock();
			_pageRepository = new PageRepositoryMock();
			_applicationSettings = new ApplicationSettings();
			
			_factory = new MarkdigParserFactory();
			_markupParser = _factory.Create(_pageRepository, _applicationSettings, _urlHelper);
		}

		private void AddPage(string title)
		{
			_pageRepository.AddNewPage(new Page() { Id = 1, Title = title }, "some text", "user", DateTime.Today);
		}

		[Test]
		public void should_configure_IMarkupParser_linkparsed_event()
		{
			// Arrange
			_urlHelper.ExpectedAction = "/wiki/1/my-page-with-spaces-in";
			var htmlLinkTag = new HtmlLinkTag("My page with spaces in", "My page with spaces in", "My link text", "_new");

			AddPage("My page with spaces in");

			// Act
			htmlLinkTag = _markupParser.LinkParsed(htmlLinkTag);

			// Assert
			Assert.That(htmlLinkTag.Href, Is.EqualTo("/wiki/1/my-page-with-spaces-in"));
		}

		[Test]
		public void should_configure_IMarkupParser_imageparsed_event()
		{
			// Arrange
			_applicationSettings.AttachmentsRoutePath = "MyAttachments";
			var htmlImageTag = new HtmlImageTag("/my.gif", "/my.gif", "alt", "title", HtmlImageTag.HorizontalAlignment.Right);

			// Act
			htmlImageTag = _markupParser.ImageParsed(htmlImageTag);

			// Assert
			Assert.That(htmlImageTag.OriginalSrc, Is.EqualTo("/my.gif"));
			Assert.That(htmlImageTag.Src, Is.EqualTo("/MyAttachments/my.gif"));
		}
	}
}