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
    public class TextPluginBeforeParseMiddlewareTests
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
        public void should_fire_beforeparse_in_textplugin()
        {
            // Arrange
            string markupFragment = "This is my ~~~usertoken~~~";
            string expectedHtml = "This is my <span>usertoken</span>";

            TextPluginStub plugin = new TextPluginStub();
            plugin.SettingsRepository = new SettingsRepositoryMock();
            plugin.PluginCache = new SiteCache(CacheMock.RoadkillCache);
            plugin.Settings.IsEnabled = true;
            _pluginFactory.RegisterTextPlugin(plugin);

            var middleware = new TextPluginBeforeParseMiddleware(_pluginRunner);
            var pageHtml = new PageHtml();
            pageHtml.Html = markupFragment;

            // Act
            PageHtml actualPageHtml = middleware.Invoke(pageHtml);

            // Assert
            Assert.That(actualPageHtml.Html, Is.EqualTo(expectedHtml));
        }
    }
}