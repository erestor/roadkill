using NUnit.Framework;
using Roadkill.Core.Text.Parsers.Links;
using Roadkill.Core.Text.Parsers.Markdig;

namespace Roadkill.Tests.Unit.Text.Parsers.Links
{
	[TestFixture]
	[Category("Unit")]
	public class MarkupLinkUpdaterTests
	{
		[Test]
		public void containspagelink_should_return_true_when_title_exists_in_markdown()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = "here is a nice [the link text](the-internal-wiki-page-title)";

			// Act
			bool hasLink = updater.ContainsPageLink(text, "the internal wiki page title");

			// Assert
			Assert.That(hasLink, Is.True);
		}

		[Test]
		public void containspagelink_should_return_false_when_title_has_no_dashes_in_markdown()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = "here is a nice [the link text](Markdown enforces dashes for spaces in urls)";

			// Act
			bool hasLink = updater.ContainsPageLink(text, "Markdown enforces dashes for spaces in urls");

			// Assert
			Assert.That(hasLink, Is.False);
		}

		[Test]
		public void containspagelink_should_return_false_when_title_does_not_exist_in_creole()
		{
			// Arrange
			var parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = "here is a nice [[the internal wiki page title|the link text]]";

			// Act
			bool hasLink = updater.ContainsPageLink(text, "page title");

			// Assert
			Assert.That(hasLink, Is.False);
		}

		[Test]
		public void containspagelink_should_return_false_when_title_does_not_exist_in_markdown()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = "here is a nice [the link text](the-internal-wiki-page-title)";

			// Act
			bool hasLink = updater.ContainsPageLink(text, "page title");

			// Assert
			Assert.That(hasLink, Is.False);
		}

		[Test]
		public void replacepagelinks_should_rename_title_inside_markdown_markup_block()
		{
			// Arrange
			var parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

            string text = @"//here is a nice **[the link text](the-internal-wiki-page-title)** and// 
                            another one: *here is a nice [the link text](the-internal-wiki-page-title) and 
							*a different one: here is a nice [the link text](different-title)";

            string expectedMarkup = @"//here is a nice **[the link text](buy-stuff-online)** and// 
                            another one: *here is a nice [the link text](buy-stuff-online) and 
							*a different one: here is a nice [the link text](different-title)";

            // Act
            string actualMarkup = updater.ReplacePageLinks(text, "the internal wiki page title", "buy stuff online");

			// Assert
			Assert.That(actualMarkup, Is.EqualTo(expectedMarkup), actualMarkup);
		}
		
		// ReplacePageLinks:
		//	- x Should rename basic creole title
		//	- x Should rename multiple creole titles
		//  - x Should rename title inside creole markup block
		//	- Should not replace title that's not found
		//  (Repeat for markdown)

		[Test]
		public void replacepagelinks_should_rename_basic_markdown_title()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = "here is a nice [the link text](the-internal-wiki-page-title)";
			string expectedMarkup = "here is a nice [the link text](buy-stuff-online)";

			// Act
			string actualMarkup = updater.ReplacePageLinks(text, "the internal wiki page title", "buy stuff online");

			// Assert
			Assert.That(actualMarkup, Is.EqualTo(expectedMarkup), actualMarkup);
		}

		[Test]
		public void replacepagelinks_should_rename_multiple_markdown_titles()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = @"here is a nice [the link text](the-internal-wiki-page-title) and 
                            another one: here is a nice [the link text](the-internal-wiki-page-title) and 
							a different one: here is a nice [the link text](different-title)";

			string expectedMarkup = @"here is a nice [the link text](buy-stuff-online) and 
                            another one: here is a nice [the link text](buy-stuff-online) and 
							a different one: here is a nice [the link text](different-title)";

			// Act
			string actualMarkup = updater.ReplacePageLinks(text, "the internal wiki page title", "buy stuff online");

			// Assert
			Assert.That(actualMarkup, Is.EqualTo(expectedMarkup), actualMarkup);
		}

		[Test]
		public void replacepagelinks_should_rename_title_inside_markdown_block()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = @"here is a nice [the link text](the-internal-wiki-page-title) and 
                            another one: here is a nice [the link text](the-internal-wiki-page-title) and 
							a different one: here is a nice [the link text](different-title)";

			string expectedMarkup = @"here is a nice [the link text](buy-stuff-online) and 
                            another one: here is a nice [the link text](buy-stuff-online) and 
							a different one: here is a nice [the link text](different-title)";

			// Act
			string actualMarkup = updater.ReplacePageLinks(text, "the internal wiki page title", "buy stuff online");

			// Assert
			Assert.That(actualMarkup, Is.EqualTo(expectedMarkup), actualMarkup);
		}

		[Test]
		public void replacepagelinks_should_not_rename_title_that_is_not_found_in_markdown()
		{
			// Arrange
			MarkdigParser parser = new MarkdigParser();
			MarkupLinkUpdater updater = new MarkupLinkUpdater(parser);

			string text = @"*here* is a nice **[the link text](the-internal-wiki-page-title)** and 
                            another one: *here is a nice [the link text](the-internal-wiki-page-title) and 
							a different one: *here is a nice [the link text](different-title)";

			// Act
			string actualMarkup = updater.ReplacePageLinks(text, "page title", "buy stuff online");

			// Assert
			Assert.That(actualMarkup, Is.EqualTo(text), actualMarkup);
		}
	}
}
