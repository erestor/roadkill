using Ganss.XSS;
using Moq;
using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.Text;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.Sanitizer;
using Roadkill.Core.Text.TextMiddleware;

namespace Roadkill.Tests.Unit.Text.TextMiddleware
{
    [TestFixture]
    [Category("Unit")]
    public class HarmfulTagMiddlewareTests
    {
        [Test]
        public void should_handle_null_sanitizer_from_factory_and_return_uncleaned_html()
        {
			// Arrange
			string html = "<div onclick=\"javascript:alert('ouch');\">test</div>";
			var pagehtml = new PageHtml() { Html = html };

	        var factoryMock = new Mock<IHtmlSanitizerFactory>();
	        factoryMock.Setup(x => x.CreateHtmlSanitizer()).Returns(() => null);

			var middleware = new HarmfulTagMiddleware(factoryMock.Object);

			// Act
			PageHtml actualPageHtml = middleware.Invoke(pagehtml);

			// Assert
			Assert.That(actualPageHtml.Html, Is.EqualTo(html));
		}

		[Test]
		public void should_clean_html_using_sanitizer()
		{
			// Arrange
			string html = "<div onclick=\"javascript:alert('ouch');\">test</div>";
			var pagehtml = new PageHtml() { Html = html };

			var htmlSanitizerMock = new Mock<IHtmlSanitizer>();
			htmlSanitizerMock.Setup(x => x.Sanitize(html, "", null)).Verifiable();

			var factoryMock = new Mock<IHtmlSanitizerFactory>();
			factoryMock.Setup(x => x.CreateHtmlSanitizer()).Returns(htmlSanitizerMock.Object);

			var middleware = new HarmfulTagMiddleware(factoryMock.Object);

			// Act
			middleware.Invoke(pagehtml);

			// Assert
			htmlSanitizerMock.Verify();
		}
    }
}