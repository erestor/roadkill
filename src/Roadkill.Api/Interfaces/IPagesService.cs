using System.Collections.Generic;
using System.Threading.Tasks;
using Roadkill.Api.Models;
using Roadkill.Core.Models;

namespace Roadkill.Api.Interfaces
{
	public interface IPagesService
	{
		/// <summary>
		/// Adds the page to the database.
		/// </summary>
		/// <param name="viewModel">The summary details for the page. The id is automatically generated for you.</param>
		/// <returns>A <see cref="PageViewModel"/> for the newly added page.</returns>
		Task<PageViewModel> Add(PageViewModel viewModel);

		/// <summary>
		/// Updates the provided page.
		/// </summary>
		/// <param name="viewModel">The page model.</param>
		Task<PageViewModel> Update(PageViewModel viewModel);

		/// <summary>
		/// Deletes a page from the database.
		/// </summary>
		/// <param name="pageId">The id of the page to remove.</param>
		Task Delete(int pageId);

		/// <summary>
		/// Retrieves the page by its id.
		/// </summary>
		/// <param name="id">The id of the page</param>
		/// <returns>A <see cref="PageVersionViewModel"/> for the page.</returns>
		Task<PageViewModel> GetById(int id);

		/// <summary>
		/// Retrieves a list of all pages in the system.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{PageViewModel}"/> of the pages.</returns>
		Task<IEnumerable<PageViewModel>> AllPages();

		/// <summary>
		/// Gets alls the pages created by a user.
		/// </summary>
		/// <param name="username">Name of the user.</param>
		/// <returns>All pages created by the provided user, or an empty list if none are found.</returns>
		Task<IEnumerable<PageViewModel>> AllPagesCreatedBy(string username);

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
	}
}