using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Roadkill.Api.Models;
using Roadkill.Core.Models;
using Roadkill.Core.Mvc.ViewModels;

namespace Roadkill.Api.Interfaces
{
	public interface IPageService
	{
		/// <summary>
		/// Adds the page to the database.
		/// </summary>
		/// <param name="model">The summary details for the page. The id is automatically generated for you.</param>
		/// <returns>A <see cref="PageViewModel"/> for the newly added page.</returns>
		Task<PageViewModel> AddPage(PageViewModel model);

		/// <summary>
		/// Retrieves a list of all pages in the system.
		/// </summary>
		/// <param name="loadPageContent">If true, includes the latest text for each page.</param>
		/// <returns>An <see cref="IEnumerable{PageViewModel}"/> of the pages.</returns>
		Task<IEnumerable<PageViewModel>> AllPages(bool loadPageContent = false);

		/// <summary>
		/// Gets alls the pages created by a user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <returns>All pages created by the provided user, or an empty list if none are found.</returns>
		Task<IEnumerable<PageViewModel>> AllPagesCreatedBy(string userName);

		/// <summary>
		/// Retrieves a list of all tags in the system.
		/// </summary>
		/// <returns>A <see cref="IEnumerable{TagViewModel}"/> for the tags.</returns>
		Task<IEnumerable<TagViewModel>> AllTags();

		/// <summary>
		/// Deletes a page from the database.
		/// </summary>
		/// <param name="pageId">The id of the page to remove.</param>
		Task DeletePage(int pageId);

		/// <summary>
		/// Exports all pages in the database, including content, to an XML format.
		/// </summary>
		/// <returns>An XML string.</returns>
		Task<string> ExportToXml();

		/// <summary>
		/// Finds all pages with the given tag.
		/// </summary>
		/// <param name="tag">The tag to search for.</param>
		/// <returns>A <see cref="IEnumerable{PageViewModel}"/> of pages tagged with the provided tag.</returns>
		Task<IEnumerable<PageViewModel>> FindByTag(string tag);

		/// <summary>
		/// Finds the first page with the tag 'homepage'. Any pages that are locked by an administrator take precedence.
		/// </summary>
		/// <returns>The homepage.</returns>
		Task<PageViewModel> FindHomePage();

		/// <summary>
		/// Finds a page by its title
		/// </summary>
		/// <param name="title">The page title</param>
		/// <returns>A <see cref="PageViewModel"/> for the page.</returns>
		Task<PageViewModel> FindByTitle(string title);

		/// <summary>
		/// Retrieves the page by its id.
		/// </summary>
		/// <param name="id">The id of the page</param>
		/// <param name="loadContent">True if the page's content should also be loaded, which will also run all text plugins.</param>
		/// <returns>A <see cref="PageVersionViewModel"/> for the page.</returns>
		Task<PageViewModel> GetById(int id, bool loadContent = false);

		/// <summary>
		/// Retrieves the current text content for a page.
		/// </summary>
		/// <param name="pageId">The id of the page.</param>
		/// <returns>The <see cref="PageVersionViewModel"/> for the page.</returns>
		Task<PageVersionViewModel> GetLatestVersion(int pageId);

		/// <summary>
		/// Renames a tag by changing all pages that reference the tag to use the new tag name.
		/// </summary>
		Task RenameTag(string oldTagName, string newTagName);

		/// <summary>
		/// Updates all links in pages to another page, when that page's title is changed.
		/// </summary>
		/// <param name="oldTitle">The previous page title.</param>
		/// <param name="newTitle">The new page title.</param>
		Task UpdateLinksToPage(string oldTitle, string newTitle);

		/// <summary>
		/// Updates the provided page.
		/// </summary>
		/// <param name="model">The page model.</param>
		Task UpdatePage(PageViewModel model);
	}
}