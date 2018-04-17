using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.Text.Parsers.Links;
using Roadkill.Core.Text.Parsers.Links.Converters;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.Text.Parsers.Links.Converters
{
	public class AttachmentLinkConverterTests
	{
		private ApplicationSettings _applicationSettings;
		private UrlHelperMock _urlHelperMock;
		private AttachmentLinkConverter _converter;

		[SetUp]
		public void Setup()
		{
			var container = new MocksAndStubsContainer();

			_applicationSettings = container.ApplicationSettings;
			_urlHelperMock = new UrlHelperMock();
			_converter = new AttachmentLinkConverter(_applicationSettings, _urlHelperMock);
		}

		[Test]
		public void IsMatch_should_return_false_for_null_link()
		{
			// Arrange
			HtmlLinkTag htmlTag = null;

			// Act
			bool actualMatch = _converter.IsMatch(htmlTag);

			// Assert
			Assert.False(actualMatch);
		}

		[Test]
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase("http://www.google.com", false)]
		[TestCase("internal-link", false)]
		[TestCase("attachment:/foo/bar.jpg", true)]
		[TestCase("~/foo/bar.jpg", true)]
		public void IsMatch_should_match_attachment_links(string href, bool expectedMatch)
		{
			// Arrange
			var htmlTag = new HtmlLinkTag(href, href, "text", "");

			// Act
			bool actualMatch = _converter.IsMatch(htmlTag);

			// Assert
			Assert.AreEqual(actualMatch, expectedMatch);
		}

		[Test]
		[TestCase("http://www.google.com", "http://www.google.com", false)]
		[TestCase("internal-link", "internal-link", false)]
		[TestCase("attachment:foo/bar.jpg", "/myattachments/foo/bar.jpg", true)]
		[TestCase("attachment:/foo/bar.jpg", "/myattachments/foo/bar.jpg", true)]
		[TestCase("~/foo/bar.jpg", "/myattachments/foo/bar.jpg", true)]
		public void Convert_should_change_expected_urls_to_full_paths(string href, string expectedHref, bool calledUrlHelper)
		{
			// Arrange
			_applicationSettings.AttachmentsRoutePath = "myattachments";
			var originalTag = new HtmlLinkTag(href, href, "text", "");

			// Act
			var actualTag = _converter.Convert(originalTag);

			// Assert
			Assert.AreEqual(actualTag.OriginalHref, originalTag.OriginalHref);
			Assert.AreEqual(actualTag.Href, expectedHref);
			Assert.AreEqual(_urlHelperMock.ContentCalled, calledUrlHelper);
		}
	}
}