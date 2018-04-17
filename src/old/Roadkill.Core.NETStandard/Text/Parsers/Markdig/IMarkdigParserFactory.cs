using Microsoft.AspNetCore.Mvc.Routing;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;

namespace Roadkill.Core.Text.Parsers.Markdig
{
	public interface IMarkdigParserFactory
	{
		// TODO: NETStandard - replace urlhelper to IUrlHelper

		MarkdigParser Create(IPageRepository pageRepository, ApplicationSettings applicationSettings, UrlHelper urlHelper);
	}
}