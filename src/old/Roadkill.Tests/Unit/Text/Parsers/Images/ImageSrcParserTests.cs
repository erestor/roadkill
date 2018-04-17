using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.Text.Parsers.Images;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.Text.Parsers.Images
{
	public class ImageSrcParserTests
	{
		private ApplicationSettings _applicationSettings;
		private ImageSrcParser _srcParser;
		private UrlHelperMock _urlHelper;

		[SetUp]
		public void Setup()
		{
			var container = new MocksAndStubsContainer();
			_applicationSettings = container.ApplicationSettings;
			_urlHelper = new UrlHelperMock();

			_srcParser = new ImageSrcParser(_applicationSettings, _urlHelper);
		}

		[Test]
		[TestCase("www.foo.com/img.jpg")]
		[TestCase("http://www.example.com/img.jpg")]
		[TestCase("https://www.foo.com/img.jpg")]
		public void should_ignore_urls_starting_with_ww_http_and_https(string imageUrl)
		{
			// Arrange
			HtmlImageTag htmlImageTag = new HtmlImageTag(imageUrl, imageUrl, "alt", "title");

			// Act
			HtmlImageTag actualTag = _srcParser.Parse(htmlImageTag);

			// Assert
			Assert.That(actualTag.Src, Is.EqualTo(imageUrl));
		}

		[Test]
		[TestCase("/DSC001.jpg", "/attuchments/DSC001.jpg")]
		public void absolute_paths_should_be_prefixed_with_attachmentpath(string path, string expectedPath)
		{
			// Arrange
			_applicationSettings.AttachmentsRoutePath = "attuchments";
			HtmlImageTag htmlImageTag = new HtmlImageTag(path, path, "alt", "title");

			// Act
			HtmlImageTag actualTag = _srcParser.Parse(htmlImageTag);

			// Assert
			Assert.That(actualTag.Src, Is.EqualTo(expectedPath));
		}
	}
}