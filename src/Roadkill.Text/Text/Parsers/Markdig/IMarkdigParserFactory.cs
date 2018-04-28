using Microsoft.AspNetCore.Mvc;
using Roadkill.Core.Repositories;

namespace Roadkill.Text.Text.Parsers.Markdig
{
    public interface IMarkdigParserFactory
    {
        // TODO: NETStandard - replace urlhelper to IUrlHelper

        MarkdigParser Create(IPageRepository pageRepository, ApplicationSettings applicationSettings, IUrlHelper urlHelper);
    }
}