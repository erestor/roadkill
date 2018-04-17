using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Roadkill.Core.DependencyResolution;
using ControllerBase = Roadkill.Core.Mvc.Controllers.ControllerBase;
using Roadkill.Core.Services;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.CoreNetCore.Localization;

namespace Roadkill.Core.Extensions
{
	// TODO: NETStandard: replace this with a TagHelper

	/// <summary>
	/// A set of extension methods for common links throughout the site.
	/// </summary>
	public static class HtmlHelperLinkExtensions
	{
		/// <summary>
		/// Gets a string to indicate whether the current user is logged in. You should render the User action rather than this extension:
		/// @Html.Action("LoggedInAs", "User")
		/// </summary>
		/// <returns>"Logged in as {user}" if the user is logged in; "Not logged in" if the user is not logged in.</returns>
		[Obsolete("You should render the User action using @Html.Action(\"LoggedInAs\", \"User\") instead of this extension")]
		public static IHtmlContent LoginStatus(this IHtmlHelper helper)
		{
			ControllerBase controller = null; // helper.ViewContext.Controller as ControllerBase;
			if (controller != null && controller.Context.IsLoggedIn)
			{
				string text = string.Format("{0} {1}", SiteStrings.Shared_LoggedInAs, controller.Context.CurrentUsername);
				return helper.ActionLink(text, "Profile", "User");
			}
			else
			{
				return new StringHtmlContent(SiteStrings.Shared_NotLoggedIn);
			}
		}

		/// <summary>
		/// Provides a link to the settings page, with optional prefix and suffix tags or seperators.
		/// </summary>
		/// <returns>If the user is not logged in and not an admin, an empty string is returned.</returns>
		public static IHtmlContent SettingsLink(this IHtmlHelper helper, string prefix, string suffix)
		{
			ControllerBase controller = null; // helper.ViewContext.Controller as ControllerBase;
			if (controller != null && controller.Context.IsAdmin)
			{
				string link = helper.ActionLink(SiteStrings.Navigation_SiteSettings, "Index", "Settings").ToString();
				return new StringHtmlContent(prefix + link + suffix);
			}
			else
			{
				return new StringHtmlContent("");
			}
		}

		/// <summary>
		/// Provides a link to the filemanager page, with optional prefix and suffix tags or seperators.
		/// </summary>
		/// <returns>If the user is not logged in, an empty string is returned.</returns>
		public static IHtmlContent FileManagerLink(this IHtmlHelper helper, string prefix, string suffix)
		{
			ControllerBase controller = null; // helper.ViewContext.Controller as ControllerBase;
			if (controller != null && (controller.Context.IsLoggedIn && (controller.Context.IsAdmin || controller.Context.IsEditor)))
			{
				string link = helper.ActionLink(SiteStrings.FileManager_Title, "Index", "FileManager").ToString();
				return new StringHtmlContent(prefix + link + suffix);
			}
			else
			{
				return new StringHtmlContent("");
			}
		}

		/// <summary>
		/// Provides a link to the login page, or if the user is logged in, the logout page.
		/// Optional prefix and suffix tags or seperators and also included.
		/// </summary>
		/// <returns>If windows authentication is being used, an empty string is returned.</returns>
		public static IHtmlContent LoginLink(this IHtmlHelper helper, string prefix, string suffix)
		{
			ControllerBase controller = null; // helper.ViewContext.Controller as ControllerBase;

			if (controller == null || controller.ApplicationSettings.UseWindowsAuthentication)
				new StringHtmlContent("");

			string link = "";

			if (controller.Context.IsLoggedIn)
			{
				link = helper.ActionLink(SiteStrings.Navigation_Logout, "Logout", "User").ToString();
			}
			else
			{
				string redirectPath = helper.ViewContext.HttpContext.Request.Path;
				link = helper.ActionLink(SiteStrings.Navigation_Login, "Login", "User", new { ReturnUrl = redirectPath }, null).ToString();

				if (controller.SettingsService.GetSiteSettings().AllowUserSignup)
					link += "&nbsp;/&nbsp;" + helper.ActionLink(SiteStrings.Navigation_Register, "Signup", "User").ToString();
			}

			return new StringHtmlContent(prefix + link + suffix);
		}

		/// <summary>
		/// Provides a link to the "new page" page, with optional prefix and suffix tags or seperators.
		/// </summary>
		/// <returns>If the user is not logged in and not an admin, an empty string is returned.</returns>
		public static IHtmlContent NewPageLink(this IHtmlHelper helper, string prefix, string suffix)
		{
			ControllerBase controller = null; // helper.ViewContext.Controller as ControllerBase;

			if (controller != null && (controller.Context.IsLoggedIn && (controller.Context.IsAdmin || controller.Context.IsEditor)))
			{
				string link = helper.ActionLink(SiteStrings.Navigation_NewPage, "New", "Pages").ToString();
				return new StringHtmlContent(prefix + link + suffix);
			}

			return new StringHtmlContent("");
		}

		/// <summary>
		/// Provides a link to the index page of the site, with optional prefix and suffix tags or seperators.
		/// </summary>
		public static IHtmlContent MainPageLink(this IHtmlHelper helper, string linkText, string prefix, string suffix)
		{
			return new StringHtmlContent(prefix + helper.ActionLink(linkText, "Index", "Home") + suffix);
		}

		/// <summary>
		/// Provides a link to the page with the provided title, querying it in the database first.
		/// </summary>
		/// <returns>If the page is not found, the link text is returned.</returns>
		public static IHtmlContent PageLink(this IHtmlHelper helper, string linkText, string pageTitle)
		{
			return helper.PageLink(linkText, pageTitle, null, "", "");
		}

		/// <summary>
		/// Provides a link to the page with the provided title, querying it in the database first,
		/// with optional prefix and suffix tags or seperators.
		/// </summary>
		/// <returns>If the page is not found, the link text is returned.</returns>
		public static IHtmlContent PageLink(this IHtmlHelper helper, string linkText, string pageTitle, string prefix, string suffix)
		{
			return helper.PageLink(linkText, pageTitle, null, prefix, suffix);
		}

		/// <summary>
		/// Provides a link to the page with the provided title, querying it in the database first,
		/// with optional prefix and suffix tags or seperators and html attributes.
		/// </summary>
		/// <param name="htmlAttributes">Any additional html attributes to add to the link</param>
		/// <returns>If the page is not found, the link text is returned.</returns>
		public static IHtmlContent PageLink(this IHtmlHelper helper, string linkText, string pageTitle, object htmlAttributes, string prefix, string suffix, IPageService pageService = null)
		{
			if (pageService == null)
				pageService = LocatorStartup.Container.GetInstance<IPageService>();

			PageViewModel model = pageService.FindByTitle(pageTitle);
			if (model != null)
			{
				string link = helper.ActionLink(linkText, "Index", "Wiki", new { id = model.Id, title = pageTitle }, htmlAttributes).ToString();
				return new StringHtmlContent(prefix + link + suffix);
			}
			else
			{
				return new StringHtmlContent(linkText);
			}
		}
	}
}