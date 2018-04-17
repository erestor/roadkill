using NUnit.Framework;
using Roadkill.Core.Text.Parsers.Links;
using Roadkill.Core.Text.Parsers.Links.Converters;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.Text.Parsers.Links.Converters
{
	public class SpecialLinkConverterTests
	{
		private UrlHelperMock _urlHelperMock;
		private SpecialLinkConverter _converter;

		[SetUp]
		public void Setup()
		{
			_urlHelperMock = new UrlHelperMock();
			_converter = new SpecialLinkConverter(_urlHelperMock);
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
		[TestCase("special:MyPage", true)]
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
		[TestCase("special:foofoo", "~/wiki/special:foofoo", true)]
		public void Convert_should_change_expected_urls_to_full_paths(string href, string expectedHref, bool calledUrlHelper)
		{
			// Arrange
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