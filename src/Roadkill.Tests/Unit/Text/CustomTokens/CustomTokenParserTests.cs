using System.IO;
using Roadkill.Text;
using Roadkill.Text.Text.CustomTokens;
using Xunit;

namespace Roadkill.Tests.Unit.Text.CustomTokens
{
    public class CustomTokenParserTests
    {
        static CustomTokenParserTests()
        {
            CustomTokenParser.CacheTokensFile = false;
        }

        [Fact]
        public void should_contain_empty_list_when_tokens_file_not_found()
        {
            // Arrange
            TextSettings settings = new TextSettings();
            settings.CustomTokensPath = Path.Combine(Directory.GetCurrentDirectory(), "Unit", "Text", "CustomTokens", "doesntexist.xml");
            CustomTokenParser parser = new CustomTokenParser(settings);

            string expectedHtml = "@@warningbox:ENTER YOUR CONTENT HERE {{some link}}@@";

            // Act
            string actualHtml = parser.ReplaceTokensAfterParse("@@warningbox:ENTER YOUR CONTENT HERE {{some link}}@@");

            // Assert
            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact]
        public void should_contain_empty_list_when_when_deserializing_bad_xml_file()
        {
            // Arrange
            TextSettings settings = new TextSettings();
            settings.CustomTokensPath = Path.Combine(Directory.GetCurrentDirectory(), "Unit", "Text", "CustomTokens", "badxml-file.json");
            string expectedHtml = "@@warningbox:ENTER YOUR CONTENT HERE {{some link}}@@";

            // Act
            CustomTokenParser parser = new CustomTokenParser(settings);
            string actualHtml = parser.ReplaceTokensAfterParse("@@warningbox:ENTER YOUR CONTENT HERE {{some link}}@@");

            // Assert
            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact]
        public void warningbox_token_should_return_html_fragment()
        {
            // Arrange
            TextSettings settings = new TextSettings();
            settings.CustomTokensPath = Path.Combine(Directory.GetCurrentDirectory(), "Unit", "Text", "CustomTokens", "customvariables.xml");
            CustomTokenParser parser = new CustomTokenParser(settings);

            string expectedHtml = @"<div class=""alert alert-warning"">ENTER YOUR CONTENT HERE {{some link}}</div><br style=""clear:both""/>";

            // Act
            string actualHtml = parser.ReplaceTokensAfterParse("@@warningbox:ENTER YOUR CONTENT HERE {{some link}}@@");

            // Assert
            Assert.Equal(expectedHtml, actualHtml);
        }
    }
}