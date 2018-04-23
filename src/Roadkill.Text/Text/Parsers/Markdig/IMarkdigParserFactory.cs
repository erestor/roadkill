using Microsoft.AspNetCore.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Repositories;

namespace Roadkill.Core.Text.Parsers.Markdig
{
    public interface IMarkdigParserFactory
    {
        // TODO: NETStandard - replace urlhelper to IUrlHelper

        MarkdigParser Create(IPageRepository pageRepository, ApplicationSettings applicationSettings, IUrlHelper urlHelper);
    }
}