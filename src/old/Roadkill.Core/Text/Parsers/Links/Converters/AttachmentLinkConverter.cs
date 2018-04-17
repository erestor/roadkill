using System.Web.Mvc;
using Roadkill.Core.Configuration;

namespace Roadkill.Core.Text.Parsers.Links.Converters
{
	public class AttachmentLinkConverter : IHtmlLinkTagConverter
	{
		private readonly ApplicationSettings _applicationSettings;
		private readonly UrlHelper _urlHelper;

		public AttachmentLinkConverter(ApplicationSettings applicationSettings, UrlHelper urlHelper)
		{
			_applicationSettings = applicationSettings;
			_urlHelper = urlHelper;
		}

		public bool IsMatch(HtmlLinkTag htmlLinkTag)
		{
			if (htmlLinkTag == null)
				return false;

			if (string.IsNullOrEmpty(htmlLinkTag.OriginalHref))
				return false;

			string lowerHref = htmlLinkTag.OriginalHref.ToLower();
			return lowerHref.StartsWith("attachment:") || lowerHref.StartsWith("~/");
		}

		public HtmlLinkTag Convert(HtmlLinkTag htmlLinkTag)
		{
			string href = htmlLinkTag.OriginalHref;
			string lowerHref = href?.ToLower() ?? "";

			if (!lowerHref.StartsWith("attachment:") && !lowerHref.StartsWith("~"))
				return htmlLinkTag;

			if (lowerHref.StartsWith("attachment:"))
			{
				href = href.Remove(0, 11);
				if (!href.StartsWith("/"))
					href = "/" + href;
			}
			else if (lowerHref.StartsWith("~/"))
			{
				// Remove the ~ 
				href = href.Remove(0, 1);
			}

			// Get the full path to the attachment
			string attachmentsPath = _applicationSettings.AttachmentsUrlPath;
			htmlLinkTag.Href = _urlHelper.Content(attachmentsPath) + href;

			return htmlLinkTag;
		}
	}
}