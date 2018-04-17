using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Roadkill.Core;
using Roadkill.Core.Cache;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Security;
using Roadkill.Core.Services;
using Roadkill.Plugins.Text.BuiltIn;
using Roadkill.Tests.Unit.StubsAndMocks;

namespace Roadkill.Tests.Unit.Plugins
{
	[TestFixture]
	[Category("Unit")]
	public class JumbotronTests
	{
		[Test]
		public void should_remove_jumbotron_tag_from_markup()
		{
			// Arrange
			string markup = "Here is some ===Heading 1=== markup \n[[[jumbotron=\n==Welcome==\n==This the subheading==]]]";
			Jumbotron jumbotron = new Jumbotron();

			// Act
			string actualMarkup = jumbotron.BeforeParse(markup);

			// Assert
			Assert.That(actualMarkup, Is.EqualTo("Here is some ===Heading 1=== markup \n"));
		}

		[Test]
		public void should_parse_and_fill_precontainerhtml()
		{
			// Arrange
			string markup = "Here is some # Heading 1\n markup \n[[[jumbotron=#Welcome\n##This is a subheading]]]";
			string expectedHtml = Jumbotron.HTMLTEMPLATE.Replace("${inner}", "<h1 id=\"welcome\">Welcome</h1>\n<h2 id=\"this-is-a-subheading\">This is a subheading</h2>\n");

			Jumbotron jumbotron = new Jumbotron();

			// Act
			jumbotron.BeforeParse(markup);
			string actualHtml = jumbotron.GetPreContainerHtml();

			// Assert
			Assert.That(actualHtml, Is.EqualTo(expectedHtml),actualHtml);
		}
	}
}
