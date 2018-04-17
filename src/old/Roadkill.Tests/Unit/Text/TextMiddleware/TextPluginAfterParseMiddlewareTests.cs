using NUnit.Framework;
using Roadkill.Core.Cache;
using Roadkill.Core.Text;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.Plugins;
using Roadkill.Core.Text.TextMiddleware;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.Text.TextMiddleware
{
    [TestFixture]
    [Category("Unit")]
    public class TextPluginAfterParseMiddlewareTests
    {
        private MocksAndStubsContainer _container;
        private TextPluginRunner _pluginRunner;
        private PluginFactoryMock _pluginFactory;

        [SetUp]
        public void Setup()
        {
            _container = new MocksAndStubsContainer();

            _pluginFactory = _container.PluginFactory;
            _pluginRunner = new TextPluginRunner(_pluginFactory);
        }

        [Test]
        public void should_fire_afterparse_in_textplugin()
        {
            // Arrange
            string markupFragment = "Here is some html <strong>bold text</strong>";
            string expectedHtml = "Here is some html <strong style='color:green'><iframe src='javascript:alert(test)'>bold text</strong>";

            TextPluginStub plugin = new TextPluginStub();
            plugin.PluginCache = new SiteCache(CacheMock.RoadkillCache);
            plugin.SettingsRepository = new SettingsRepositoryMock();
            plugin.Settings.IsEnabled = true;
            _pluginFactory.RegisterTextPlugin(plugin);

            var middleware = new TextPluginAfterParseMiddleware(_pluginRunner);
            var pageHtml = new PageHtml();
            pageHtml.Html = markupFragment;

            // Act
            PageHtml actualPageHtml = middleware.Invoke(pageHtml);

            // Assert
            Assert.That(actualPageHtml.Html, Is.EqualTo(expectedHtml));
        }

        [Test]
        public void should_set_pre_and_post_container_html()
        {
            // Arrange
            TextPluginStub plugin = new TextPluginStub();
            plugin.PluginCache = new SiteCache(CacheMock.RoadkillCache);
            plugin.SettingsRepository = new SettingsRepositoryMock();
            plugin.Settings.IsEnabled = true;
            plugin.PreContainerHtml = "pre container html";
            plugin.PostContainerHtml = "post container html";
            _pluginFactory.RegisterTextPlugin(plugin);

            var middleware = new TextPluginAfterParseMiddleware(_pluginRunner);
            var pageHtml = new PageHtml();

            // Act
            PageHtml actualPageHtml = middleware.Invoke(pageHtml);

            // Assert
            Assert.That(actualPageHtml.PreContainerHtml, Is.EqualTo(plugin.PreContainerHtml));
            Assert.That(actualPageHtml.PostContainerHtml, Is.EqualTo(plugin.PostContainerHtml));
        }

        [Test]
        public void should_set_cacheable_from_runner_value()
        {
            // Arrange
            TextPluginStub plugin = new TextPluginStub();
            plugin.SettingsRepository = new SettingsRepositoryMock();
            plugin.PluginCache = new SiteCache(CacheMock.RoadkillCache);
            plugin.IsCacheable = true;
            plugin.Settings.IsEnabled = true;
            _pluginFactory.RegisterTextPlugin(plugin);

            var middleware = new TextPluginAfterParseMiddleware(_pluginRunner);
            var pageHtml = new PageHtml();

            // Act
            PageHtml actualPageHtml = middleware.Invoke(pageHtml);

            // Assert
            Assert.That(actualPageHtml.IsCacheable, Is.EqualTo(plugin.IsCacheable));
        }
    }
}