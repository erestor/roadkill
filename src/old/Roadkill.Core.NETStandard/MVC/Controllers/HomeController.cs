using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Core.Configuration;
using Roadkill.Core.Services;
using Roadkill.Core.Mvc.Attributes;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Security;
using Roadkill.Core.Text.TextMiddleware;
using Roadkill.CoreNetCore.Localization;

namespace Roadkill.Core.Mvc.Controllers
{
	/// <summary>
	/// Provides functionality that is common through the site.
	/// </summary>
	[OptionalAuthorization]
	public class HomeController : ControllerBase
	{
		public IPageService PageService { get; private set; }
		private SearchService _searchService;
		private readonly TextMiddlewareBuilder _textMiddlewareBuilder;

		public HomeController(ApplicationSettings settings, UserServiceBase userManager, IPageService pageService,
			SearchService searchService, IUserContext context, SettingsService settingsService, TextMiddlewareBuilder textMiddlewareBuilder)
			: base(settings, userManager, context, settingsService)
		{
			_searchService = searchService;
			_textMiddlewareBuilder = textMiddlewareBuilder;
			PageService = pageService;
		}

		/// <summary>
		/// Display the homepage/mainpage. If no page has been tagged with the 'homepage' tag,
		/// then a dummy PageViewModel is put in its place.
		/// </summary>
		[BrowserCache]
		public ActionResult Index()
		{
			// Get the first locked homepage
			PageViewModel model = PageService.FindHomePage();

			if (model == null)
			{
				model = new PageViewModel();
				model.Title = SiteStrings.NoMainPage_Title;
				model.Content = SiteStrings.NoMainPage_Label;
				model.ContentAsHtml = _textMiddlewareBuilder.Execute(SiteStrings.NoMainPage_Label).Html;
				model.CreatedBy = "";
				model.CreatedOn = DateTime.UtcNow;
				model.RawTags = "homepage";
				model.ModifiedOn = DateTime.UtcNow;
				model.ModifiedBy = "";
			}

			return View(model);
		}

		/// <summary>
		/// Searches the lucene index using the search string provided.
		/// </summary>
		public ActionResult Search(string q)
		{
			ViewData["search"] = q;

			List<SearchResultViewModel> results = _searchService.Search(q).ToList();
			return View(results);
		}

		/// <summary>
		/// Returns Javascript 'constants' for the site.
		/// </summary>
		/// <param name="version">This is sent by the views to ensure new versions of Roadkill have this JS file cleared from the cache.</param>

		// TODO: NETStandard - find a way of caching
		//[CacheContentType(Duration = 86400 * 30, ContentType = "application/javascript")] // 30 days
		[AllowAnonymous]
		public ActionResult GlobalJsVars(string version)
		{
			return View();
		}

		/// <summary>
		/// Displays the left side menu view, including new page/settings if logged in.
		/// </summary>
		[AllowAnonymous]
		public ActionResult NavMenu()
		{
			return Content(PageService.GetMenu(Context));
		}

		/// <summary>
		/// Displays the a Bootstrap-styled left side menu view, including new page/settings if logged in.
		/// </summary>
		[AllowAnonymous]
		public ActionResult BootstrapNavMenu()
		{
			var pageService = PageService as PageService;
			if (pageService == null)
				return Content("");

			return Content(pageService.GetBootStrapNavMenu(Context));
		}
	}
}