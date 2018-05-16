using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Api.Interfaces;
using Roadkill.Api.Models;
using Roadkill.Core.Models;
using Roadkill.Core.Mvc.ViewModels;
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
		public async Task<PageViewModel> Add(PageViewModel model)
		{
			throw new NotImplementedException();
		}

		[HttpPut]
		public async Task Update(PageViewModel model)
		{
			throw new NotImplementedException();
		}

		[HttpDelete]
		public async Task Delete(int pageId)
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		public async Task<PageViewModel> GetById(int id)
		{
			throw new NotImplementedException();
		}

		[Route("AllPages")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> AllPages(bool loadPageContent = false)
		{
			var allpages = await _pageRepository.AllPages();
			return allpages.Select(_pageViewModelConverter.CreateViewModel);
		}

		[Route("AllPagesCreatedBy")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> AllPagesCreatedBy(string username)
		{
			var pagesCreatedBy = await _pageRepository.FindPagesCreatedBy(username);
			return pagesCreatedBy.Select(_pageViewModelConverter.CreateViewModel);
		}

		[Route("FindHomePage")]
		[HttpGet]
		public async Task<PageViewModel> FindHomePage()
		{
			throw new NotImplementedException();
		}

		[Route("FindByTitle")]
		[HttpGet]
		public async Task<PageViewModel> FindByTitle(string title)
		{
			throw new NotImplementedException();
		}
	}
}