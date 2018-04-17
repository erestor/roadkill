using System;
using NUnit.Framework;
using Roadkill.Core.DependencyResolution.StructureMap;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.TextMiddleware;
using Roadkill.Tests.Unit.StubsAndMocks;
using StructureMap;

namespace Roadkill.Tests.Unit.Text.TextMiddleware
{
    public class TextMiddlewareBuilderTests
    {
        private TextMiddlewareBuilder CreateFullBuilder()
        {
            var registry = new TextRegistry();
            var container = new Container(c =>
            {
                c.AddRegistry(registry);
            });

            return container.GetInstance<TextMiddlewareBuilder>();
        }

        [Test]
        public void should()
        {
            // given
            var builder = CreateFullBuilder();

            // when
            PageHtml pageHtml = builder.Execute("![Image title](/DSC001.jpg)");

            // then
            Console.WriteLine(pageHtml);
        }

        [Test]
        public void use_should_throw_when_middleware_is_null()
        {
            // given, when
            string markup = "";
            var builder = new TextMiddlewareBuilder();

            // then
            Assert.Throws<ArgumentNullException>(() => builder.Use(null));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo("item1 item2"));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo("value1 value2"));
        }
    }
}
