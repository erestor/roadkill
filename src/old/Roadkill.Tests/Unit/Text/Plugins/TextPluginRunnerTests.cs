using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Roadkill.Core.Text.Plugins;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.Text.Plugins
{
	public class TextPluginRunnerTests
	{
		private TextPluginRunner _textPluginRunner;
		private PluginFactoryMock _pluginFactory;

		[SetUp]
		public void Setup()
		{
			_pluginFactory = new PluginFactoryMock();
			_textPluginRunner = new TextPluginRunner(_pluginFactory);
		}

		[Test]
		public void ctor_should_throw_if_factory_is_null()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void ctor_should_populate_plugins_from_factory()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void BeforeParse_should_append_head_and_footer_html_from_plugins()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void BeforeParse_should_cache_response()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void AfterParse_should_append_html_from_plugins()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void AfterParse_should_cache_response()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void BeforeParse_should_set_header_and_footer_html_from_plugins()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void PreContainerHtml_should_append_html_from_plugins()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}

		[Test]
		public void PostContainerHtml_should_append_html_from_plugins()
		{
			// given

			// when

			// then
			Assert.Fail("todo");
		}
	}
}