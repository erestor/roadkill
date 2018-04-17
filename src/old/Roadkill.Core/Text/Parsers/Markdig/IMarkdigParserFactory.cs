using System.Web.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;

namespace Roadkill.Core.Text.Parsers.Markdig
{
	public interface IMarkdigParserFactory
	{
		MarkdigParser Create(IPageRepository pageRepository, ApplicationSettings applicationSettings, UrlHelper urlHelper);
	}
}