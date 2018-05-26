using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Api.Interfaces;
using Roadkill.Api.Models;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;

namespace Roadkill.Api.Controllers
{
	[Route("pages")]
	public class PagesController : Controller, IPagesService
	{
		private readonly IPageRepository _pageRepository;
		private readonly IPageViewModelConverter _pageViewModelConverter;

		public PagesController(IPageRepository pageRepository, IPageViewModelConverter pageViewModelConverter)
		{
			_pageRepository = pageRepository;
			_pageViewModelConverter = pageViewModelConverter;
		}

		[HttpPost]
		public async Task<PageViewModel> Add(PageViewModel viewModel)
		{
			Page page = _pageViewModelConverter.ConvertToPage(viewModel);
			if (page == null)
				return null;

			Page newPage = await _pageRepository.AddNewPage(page);
			return _pageViewModelConverter.ConvertToViewModel(newPage);
		}

		[HttpPut]
		public async Task<PageViewModel> Update(PageViewModel viewModel)
		{
			Page page = _pageViewModelConverter.ConvertToPage(viewModel);
			if (page == null)
				return null;

			Page newPage = await _pageRepository.UpdateExisting(page);
			return _pageViewModelConverter.ConvertToViewModel(newPage);
		}

		[HttpDelete]
		public async Task Delete(int pageId)
		{
			await _pageRepository.DeletePage(pageId);
		}

		[HttpGet]
		[Route("Get")]
		public async Task<PageViewModel> GetById(int id)
		{
			Page page = await _pageRepository.GetPageById(id);
			if (page == null)
				return null;

			return _pageViewModelConverter.ConvertToViewModel(page);
		}

		[HttpGet]
		[Route(nameof(AllPages))]
		public async Task<IEnumerable<PageViewModel>> AllPages()
		{
			IEnumerable<Page> allpages = await _pageRepository.AllPages();
			return allpages.Select(_pageViewModelConverter.ConvertToViewModel);
		}

		[HttpGet]
		[Route(nameof(AllPagesCreatedBy))]
		public async Task<IEnumerable<PageViewModel>> AllPagesCreatedBy(string username)
		{
			IEnumerable<Page> pagesCreatedBy = await _pageRepository.FindPagesCreatedBy(username);
			return pagesCreatedBy.Select(_pageViewModelConverter.ConvertToViewModel);
		}

		[HttpGet]
		[Route(nameof(FindHomePage))]
		public async Task<PageViewModel> FindHomePage()
		{
			IEnumerable<Page> pagesWithHomePageTag = await _pageRepository.FindPagesContainingTag("homepage");

			if (!pagesWithHomePageTag.Any())
				return null;

			Page firstResult = pagesWithHomePageTag.First();
			return _pageViewModelConverter.ConvertToViewModel(firstResult);
		}

		[HttpGet]
		[Route(nameof(FindByTitle))]
		public async Task<PageViewModel> FindByTitle(string title)
		{
			Page page = await _pageRepository.GetPageByTitle(title);
			if (page == null)
				return null;

			return _pageViewModelConverter.ConvertToViewModel(page);
		}
	}
}