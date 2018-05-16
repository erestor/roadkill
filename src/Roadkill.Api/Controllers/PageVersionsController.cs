using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Api.Interfaces;
using Roadkill.Core.Models;
using Roadkill.Core.Repositories;

namespace Roadkill.Api.Controllers
{
	[Route("tags")]
	public class PageVersionsController : Controller, IPageVersionsService
	{
		private readonly IPageRepository _pageRepository;

		public PageVersionsController(IPageRepository pageRepository)
		{
			_pageRepository = pageRepository;
		}

		[Route("UpdateLinksToPage")]
		[HttpPost]
		public async Task UpdateLinksToPage(string oldTitle, string newTitle)
		{
			throw new NotImplementedException();
		}

		[Route("GetLatestVersion")]
		[HttpGet]
		public async Task<PageVersionViewModel> GetLatestVersion(int pageId)
		{
			throw new NotImplementedException();
		}
	}
}