using Xunit;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.Parsers.Markdig;
using Roadkill.Core.Text.TextMiddleware;

namespace Roadkill.Tests.Unit.Text.TextMiddleware
{
    public class MarkupParserMiddlewareTests
    {
        [Fact]
        public void should_parser_markup_using_parser()
        {
            // Arrange
            string markdown = "some **bold** text";
            string expectedHtml = "<p>some <strong>bold</strong> text</p>\n";

            var pagehtml = new PageHtml() { Html = markdown };

            var parser = new MarkdigParser();
            var middleware = new MarkupParserMiddleware(parser);

            // Act
            PageHtml actualPageHtml = middleware.Invoke(pagehtml);

            // Assert
            Assert.Equal(expectedHtml, actualPageHtml.Html);
        }
    }
}