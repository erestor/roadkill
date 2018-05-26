using System.Collections.Generic;
using System.Threading.Tasks;
using Roadkill.Api.Models;

namespace Roadkill.Api.Interfaces
{
	public interface IPagesService
	{
		Task<PageViewModel> Add(PageViewModel viewModel);

		Task<PageViewModel> Update(PageViewModel viewModel);

		Task Delete(int pageId);

		Task<PageViewModel> GetById(int id);

		Task<IEnumerable<PageViewModel>> AllPages();

		Task<IEnumerable<PageViewModel>> AllPagesCreatedBy(string username);

		Task<PageViewModel> FindHomePage();

		Task<PageViewModel> FindByTitle(string title);
	}
}