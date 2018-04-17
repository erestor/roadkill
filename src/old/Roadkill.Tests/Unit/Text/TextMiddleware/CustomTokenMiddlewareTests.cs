using System;
using System.IO;
using NUnit.Framework;
using Roadkill.Core;
using Roadkill.Core.Configuration;
using Roadkill.Core.Text;
using Roadkill.Core.Text.CustomTokens;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.TextMiddleware;

namespace Roadkill.Tests.Unit.Text.TextMiddleware
{
    [TestFixture]
    [Category("Unit")]
    public class CustomTokenMiddlewareTests
    {
        [Test]
        public void should_clean_html_using_sanitizer()
        {
            string markdown = @"@@warningbox:ENTER YOUR CONTENT HERE 

here is some more content


@@";

            string expectedHtml = @"<div class=""alert alert-warning"">ENTER YOUR CONTENT HERE 

here is some more content

</div><br style=""clear:both""/>";


            var pagehtml = new PageHtml() { Html = markdown };

            var appSettings = new ApplicationSettings();
            appSettings.CustomTokensPath = Path.Combine(TestConstants.WEB_PATH, "App_Data", "customvariables.xml");

            var customTokenParser = new CustomTokenParser(appSettings);
            var middleware = new CustomTokenMiddleware(customTokenParser);

            // Act
            PageHtml actualPageHtml = middleware.Invoke(pagehtml);

            actualPageHtml.Html = actualPageHtml.Html.Replace(Environment.NewLine, "");
            expectedHtml = expectedHtml.Replace(Environment.NewLine, "");

            // Assert
            Assert.That(actualPageHtml.Html, Is.EqualTo(expectedHtml), actualPageHtml.Html);
        }
    }
}