using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Roadkill.Api.Models;
using Roadkill.Core.Models;

namespace Roadkill.Api.Interfaces
{
	public interface IPageVersionsService
	{
		Task<PageVersionViewModel> Add(int pageId, string text, string author, DateTime? dateTime = null);

		Task<PageVersionViewModel> GetById(Guid id);

		Task Delete(Guid id);

		Task Update(PageVersionViewModel version);

		/// <summary>
		/// Retrieves the current text content for a page.
		/// </summary>
		/// <param name="pageId">The id of the page.</param>
		/// <returns>The <see cref="PageVersionViewModel"/> for the page.</returns>
		Task<PageVersionViewModel> GetLatestVersion(int pageId);

		/// <summary>
		/// Updates all links in a page to point to the new page's title.
		/// </summary>
		/// <param name="oldTitle">The previous page title.</param>
		/// <param name="newTitle">The new page title.</param>
		Task UpdateLinksToPage(string oldTitle, string newTitle);

		Task<IEnumerable<PageVersionViewModel>> AllVersions();

		Task<IEnumerable<PageVersionViewModel>> FindPageVersionsByPageId(int pageId);

		Task<IEnumerable<PageVersionViewModel>> FindPageVersionsByAuthor(string username);
	}
}