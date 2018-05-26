using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Roadkill.Api.Models;

namespace Roadkill.Api.Interfaces
{
	public interface IPageVersionsService
	{
		Task<PageVersionViewModel> Add(int pageId, string text, string author, DateTime? dateTime = null);

		Task<PageVersionViewModel> GetById(Guid id);

		Task Delete(Guid id);

		Task Update(PageVersionViewModel pageVersionViewModel);

		Task<PageVersionViewModel> GetLatestVersion(int pageId);

		Task<IEnumerable<PageVersionViewModel>> AllVersions();

		Task<IEnumerable<PageVersionViewModel>> FindPageVersionsByPageId(int pageId);

		Task<IEnumerable<PageVersionViewModel>> FindPageVersionsByAuthor(string username);
	}
}