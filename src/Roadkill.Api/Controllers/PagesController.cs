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
	public class PagesController : Controller, IPageService
	{
		private readonly IPageRepository _pageRepository;

		public PagesController(IPageRepository pageRepository)
		{
			_pageRepository = pageRepository;
		}

		[HttpPost]
		public async Task<PageViewModel> AddPage(PageViewModel model)
		{
			throw new NotImplementedException();
		}

		[Route("AllPages")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> AllPages(bool loadPageContent = false)
		{
			var allpages = await _pageRepository.AllPages();

			var converter = new PageViewModelConverter();
			return allpages.Select(converter.Create);
		}

		[Route("AllPagesCreatedBy")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> AllPagesCreatedBy(string userName)
		{
			throw new NotImplementedException();
		}

		[Route("AllTags")]
		[HttpGet]
		public async Task<IEnumerable<TagViewModel>> AllTags()
		{
			throw new NotImplementedException();
		}

		[Route("")]
		[HttpDelete]
		public async Task DeletePage(int pageId)
		{
			throw new NotImplementedException();
		}

		[Route("ExportToXml")]
		[HttpGet]
		public async Task<string> ExportToXml()
		{
			throw new NotImplementedException();
		}

		[Route("FindByTag")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> FindByTag(string tag)
		{
			throw new NotImplementedException();
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

		[Route("GetById")]
		[HttpGet]
		public async Task<PageViewModel> GetById(int id, bool loadContent = false)
		{
			throw new NotImplementedException();
		}

		[Route("GetLatestVersion")]
		[HttpGet]
		public async Task<PageVersionViewModel> GetLatestVersion(int pageId)
		{
			throw new NotImplementedException();
		}

		[Route("RenameTag")]
		[HttpPost]
		public async Task RenameTag(string oldTagName, string newTagName)
		{
			throw new NotImplementedException();
		}

		[Route("UpdateLinksToPage")]
		[HttpPost]
		public async Task UpdateLinksToPage(string oldTitle, string newTitle)
		{
			throw new NotImplementedException();
		}

		[HttpPut]
		public async Task UpdatePage(PageViewModel model)
		{
			throw new NotImplementedException();
		}
	}
}