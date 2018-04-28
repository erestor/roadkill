using System;
using Moq;
using Roadkill.Text;
using Roadkill.Text.Text.CustomTokens;
using Roadkill.Text.Text.Menu;
using Roadkill.Text.Text.Plugins;
using Roadkill.Text.Text.Sanitizer;
using Roadkill.Text.Text.TextMiddleware;
using Xunit;

namespace Roadkill.Tests.Unit.Text.TextMiddleware
{
    public class TextMiddlewareBuilderTests
    {
        public class MiddleWareMock : Middleware
        {
            public string SearchString { get; set; }
            public string Replacement { get; set; }

            public override PageHtml Invoke(PageHtml pageHtml)
            {
                return pageHtml.Html.Replace(SearchString, Replacement);
            }
        }

        private TextMiddlewareBuilder CreateFullBuilder()
        {
            var builder = new TextMiddlewareBuilder();
            builder.Use(new CustomTokenMiddleware(new CustomTokenParser(new ApplicationSettings())))
                   .Use(new HarmfulTagMiddleware(new HtmlSanitizerFactory(new ApplicationSettings())))
                   .Use(new TextPluginAfterParseMiddleware(new TextPluginRunner()));

            return builder;
        }

        [Fact]
        public void should()
        {
            // given
            var builder = CreateFullBuilder();

            // when
            PageHtml pageHtml = builder.Execute("![Image title](/DSC001.jpg)");

            // then
            Console.WriteLine(pageHtml);
        }

        [Fact]
        public void use_should_throw_when_middleware_is_null()
        {
            // given, when
            string markup = "";
            var builder = new TextMiddlewareBuilder();

            // then
            Assert.Throws<ArgumentNullException>(() => builder.Use(null));
        }

        [Fact]
        public void execute_should_swallow_exceptions()
        {
            // given
            string markup = "item1 item2";
            var builder = new TextMiddlewareBuilder();
            var middleware1 = new MiddleWareMock() { SearchString = null, Replacement = null };

            builder.Use(middleware1);

            // when
            string result = builder.Execute(markup);

            // then
            Assert.Equal("item1 item2", result);
        }

        [Fact]
        public void use_should_add_middleware_and_execute_should_concatenate_values_from_middleware()
        {
            // given
            string markup = "item1 item2";
            var builder = new TextMiddlewareBuilder();
            var middleware1 = new MiddleWareMock() { SearchString = "item1", Replacement = "value1" };
            var middleware2 = new MiddleWareMock() { SearchString = "item2", Replacement = "value2" };

            builder.Use(middleware1);
            builder.Use(middleware2);

            // when
            string result = builder.Execute(markup);

            // then
            Assert.Equal("value1 value2", result);
        }
    }
}