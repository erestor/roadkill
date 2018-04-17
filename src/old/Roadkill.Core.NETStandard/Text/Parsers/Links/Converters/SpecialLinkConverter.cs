using Microsoft.AspNetCore.Mvc.Routing;

namespace Roadkill.Core.Text.Parsers.Links.Converters
{
	public class SpecialLinkConverter : IHtmlLinkTagConverter
	{
		// TODO: NETStandard - replace urlhelper to IUrlHelper
		private readonly UrlHelper _urlHelper;

		public SpecialLinkConverter(UrlHelper urlHelper)
		{
			_urlHelper = urlHelper;
		}

		public bool IsMatch(HtmlLinkTag htmlLinkTag)
		{
			if (htmlLinkTag == null)
				return false;

			string href = htmlLinkTag.OriginalHref;
			string lowerHref = href?.ToLower() ?? "";

			return lowerHref.StartsWith("special:");
		}

		public HtmlLinkTag Convert(HtmlLinkTag htmlLinkTag)
		{
			string href = htmlLinkTag.OriginalHref;
			string lowerHref = href?.ToLower();

			if (!lowerHref.StartsWith("special:"))
				return htmlLinkTag;

			htmlLinkTag.Href = _urlHelper.Content("~/wiki/" + href);

			return htmlLinkTag;
		}
	}
}