using System.Web.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Text.Parsers.Images;
using Roadkill.Core.Text.Parsers.Links;

namespace Roadkill.Core.Text.Parsers.Markdig
{
	public class MarkdigParserFactory : IMarkdigParserFactory
	{
		public MarkdigParser Create(IPageRepository pageRepository, ApplicationSettings applicationSettings, UrlHelper urlHelper)
		{
			var markdigParser = new MarkdigParser();

			// When a link is parsed, use the LinkHrefParser
			markdigParser.LinkParsed = htmlLinkTag =>
			{
				var tokenParser = new LinkHrefParser(pageRepository, applicationSettings, urlHelper);
				htmlLinkTag = tokenParser.Parse(htmlLinkTag);

				return htmlLinkTag;
			};

			// When an image is parsed, use the ImageSrcParser
			markdigParser.ImageParsed = htmlImageTag =>
			{
				var provider = new ImageSrcParser(applicationSettings, urlHelper);
				htmlImageTag = provider.Parse(htmlImageTag);

				return htmlImageTag;
			};

			return markdigParser;
		}
	}
}