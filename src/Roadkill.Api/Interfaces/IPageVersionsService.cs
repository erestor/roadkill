using System.Threading.Tasks;
using Roadkill.Core.Models;

namespace Roadkill.Api.Interfaces
{
	public interface IPageVersionsService
	{
		/// <summary>
		/// Retrieves the current text content for a page.
		/// </summary>
		/// <param name="pageId">The id of the page.</param>
		/// <returns>The <see cref="PageVersionViewModel"/> for the page.</returns>
		Task<PageVersionViewModel> GetLatestVersion(int pageId);

		/// <summary>
		/// Updates all links in pages to another page, when that page's title is changed.
		/// </summary>
		/// <param name="oldTitle">The previous page title.</param>
		/// <param name="newTitle">The new page title.</param>
		Task UpdateLinksToPage(string oldTitle, string newTitle);
	}
}