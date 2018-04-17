using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Mvc.ViewModels;

namespace Roadkill.Core.Text.Parsers.Links
{
	/// <summary>
	/// Parses:
	///   - Tokens inside links, e.g. special:, attachment:
	///   - Internal links.
	///   - CSS classes for external.
	///   - Broken links.
	/// </summary>
	public class LinkHrefParser
	{
		private readonly IPageRepository _pageRepository;
		private readonly ApplicationSettings _applicationSettings;
		private readonly UrlHelper _urlHelper;
		private readonly List<string> _externalLinkPrefixes = new List<string>()
		{
			"http://",
			"https://",
			"www.",
			"mailto:",
			"#",
			"tag:"
		};

		private static readonly Regex _querystringRegex = new Regex("(?<querystring>(\\?).+)", RegexOptions.IgnoreCase);
		private static readonly Regex _anchorRegex = new Regex("(?<hash>(#|%23).+)", RegexOptions.IgnoreCase);

		public LinkHrefParser(IPageRepository pageRepository, ApplicationSettings applicationSettings, UrlHelper urlHelper)
		{
			if (pageRepository == null)
				throw new ArgumentNullException(nameof(pageRepository));

			if (applicationSettings == null)
				throw new ArgumentNullException(nameof(applicationSettings));

			_pageRepository = pageRepository;
			_applicationSettings = applicationSettings;
			_urlHelper = urlHelper;
		}

		public HtmlLinkTag Parse(HtmlLinkTag htmlLinkTag)
		{
			if (IsExternalLink(htmlLinkTag.OriginalHref))
			{
				// Add the external-link class to all outward bound links, 
				// except for anchors pointing to <a name=""> tags on the current page.
				// (# links shouldn't be treated as internal links)
				if (!htmlLinkTag.OriginalHref.StartsWith("#"))
					htmlLinkTag.CssClass = "external-link";
			}
			else
			{
				string href = htmlLinkTag.OriginalHref;
				string lowerHref = href.ToLower();

				if (lowerHref.StartsWith("attachment:") || lowerHref.StartsWith("~/"))
				{
					ConvertAttachmentToFullPath(htmlLinkTag);
				}
				else if (lowerHref.StartsWith("special:"))
				{
					ConvertSpecialLinkToFullPath(htmlLinkTag);
				}
				else
				{
					ConvertInternalLinkToFullPath(htmlLinkTag);
				}
			}

			return htmlLinkTag;
		}

		private bool IsExternalLink(string href)
		{
			return _externalLinkPrefixes.Any(x => href.StartsWith(x));
		}

		private void ConvertAttachmentToFullPath(HtmlLinkTag htmlLinkTag)
		{
			string href = htmlLinkTag.OriginalHref;
			string lowerHref = href.ToLower();

			if (lowerHref.StartsWith("attachment:"))
			{
				// Remove the attachment: part
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
			htmlLinkTag.Href = ConvertToAbsolutePath(attachmentsPath) + href;
		}

		private void ConvertSpecialLinkToFullPath(HtmlLinkTag htmlLinkTag)
		{
			// note: "~" is replaced by ASP.NET HttpContext
			string href = htmlLinkTag.OriginalHref;
			htmlLinkTag.Href = ConvertToAbsolutePath("~/wiki/" + href);
		}

		private string ConvertToAbsolutePath(string relativeUrl)
		{
			// e.g. ~/mydir/page1.html to /mywiki/mydir/page1.html.
			return _urlHelper.Content(relativeUrl);
		}

		private void ConvertInternalLinkToFullPath(HtmlLinkTag htmlLinkTag)
		{
			string href = htmlLinkTag.OriginalHref;

			// Parse internal links
			string title = href;
			string querystringAndAnchor = ""; // querystrings, #anchors

			// Parse querystrings and #
			if (_querystringRegex.IsMatch(href))
			{
				// Grab the querystring contents
				System.Text.RegularExpressions.Match match = _querystringRegex.Match(href);
				querystringAndAnchor = match.Groups["querystring"].Value;

				// Grab the url
				title = href.Replace(querystringAndAnchor, "");
			}
			else if (_anchorRegex.IsMatch(href))
			{
				// Grab the hash contents
				System.Text.RegularExpressions.Match match = _anchorRegex.Match(href);
				querystringAndAnchor = match.Groups["hash"].Value;

				// Grab the url
				title = href.Replace(querystringAndAnchor, "");
			}

			// For markdown, only urls with "-" in them are valid, spaces are ignored.
			// Remove these, so a match is made. No page title should have a "-" in?, so replacing them is ok.
			title = title.Replace("-", " ");

			// Find the page, or if it doesn't exist point to the new page url
			Page page = _pageRepository.GetPageByTitle(title);
			if (page != null)
			{
				string encodedTitle = PageViewModel.EncodePageTitle(page.Title);
				href = _urlHelper.Action("Index", "Wiki", new { id = page.Id, title = encodedTitle });
				href += querystringAndAnchor;
			}
			else
			{
				// e.g. /pages/new?title=xyz
				href = _urlHelper.Action("New", "Pages", new { title = title });
				htmlLinkTag.CssClass = "missing-page-link";
			}

			htmlLinkTag.Href = href;
			htmlLinkTag.Target = "";
		}
	}
}