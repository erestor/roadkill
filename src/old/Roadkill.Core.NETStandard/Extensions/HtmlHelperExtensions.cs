using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Roadkill.Core.Services;
using ControllerBase = Roadkill.Core.Mvc.Controllers.ControllerBase;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Mvc.Controllers;

namespace Roadkill.Core.Extensions
{
	/// <summary>
	/// Roadkill specific extensions methods for the <see cref="IHtmlHelper"/> class.
	/// </summary>
	public static class IHtmlHelperExtensions
	{
		/// <summary>
		/// Creates a drop down list from an <c>IDictionary</c> and selects the item.
		/// </summary>
		public static IHtmlContent DropDownBox(this IHtmlHelper helper, string name, IDictionary<string, string> items, string selectedValue)
		{
			List<SelectListItem> selectList = new List<SelectListItem>();

			foreach (string key in items.Keys)
			{
				SelectListItem selectListItem = new SelectListItem
				{
					Text = items[key],
					Value = key
				};

				if (key == selectedValue)
					selectListItem.Selected = true;

				selectList.Add(selectListItem);
			}

			return helper.DropDownList(name, selectList);
		}

		/// <summary>
		/// Creates a drop down list from an <c>IList</c> of strings.
		/// </summary>
		public static IHtmlContent DropDownBox(this IHtmlHelper helper, string name, IEnumerable<string> items)
		{
			List<SelectListItem> selectList = new List<SelectListItem>();

			foreach (string item in items)
			{
				SelectListItem selectListItem = new SelectListItem
				{
					Text = item,
					Value = item
				};

				selectList.Add(selectListItem);
			}

			return helper.DropDownList(name, selectList, new { id = name });
		}

		/// <summary>
		/// Render the first page which has this tag. Admin locked pages have priority.
		/// </summary>
		/// <param name="tag">the tagname</param>
		/// <returns>html</returns>
		/// <example>
		/// usage:   @Html.RenderPageByTag("secondMenu")
		/// </example>
		public static IHtmlContent RenderPageByTag(this IHtmlHelper helper, string tag)
		{
			string html = "";

			// TODO: NETStandard: replace this with a TagHelper
			ControllerBase controller = null; //helper.ViewContext.Controller as ControllerBase;
			WikiController wikiController = controller as WikiController;
			if (wikiController != null)
			{
				PageService pageService = wikiController.PageService;

				IEnumerable<PageViewModel> pages = pageService.FindByTag(tag);
				if (pages.Count() > 0)
				{
					// Find the page, first search for a locked page.
					PageViewModel model = pages.FirstOrDefault(h => h.IsLocked);
					if (model == null)
					{
						model = pages.FirstOrDefault();
					}

					if (model != null)
					{
						html = model.ContentAsHtml;
					}
				}
			}

			return new StringHtmlContent(html);
		}

		/// <summary>
		/// An alias for Partial() to indicate a dialog's HTML is being rendered.
		/// </summary>
		public static IHtmlContent DialogPartial(this IHtmlHelper helper, string viewName)
		{
			return helper.Partial("Dialogs/" + viewName);
		}

		/// <summary>
		/// An alias for Partial() to indicate a dialog's HTML is being rendered.
		/// </summary>
		public static IHtmlContent DialogPartial(this IHtmlHelper helper, string viewName, object model)
		{
			return helper.Partial(viewName, model);
		}

		/// <summary>
		/// Returns the rendered partial navigation menu.
		/// </summary>
		public static IHtmlContent SiteSettingsNavigation(this IHtmlHelper IHtmlHelper)
		{
			return IHtmlHelper.Partial("Navigation");
		}
	}
}