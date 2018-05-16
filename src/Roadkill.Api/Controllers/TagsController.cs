using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Api.Interfaces;
using Roadkill.Api.Models;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Repositories;

namespace Roadkill.Api.Controllers
{
	[Route("tags")]
	public class TagsController : Controller, ITagsService
	{
		private readonly IPageRepository _pageRepository;

		public TagsController(IPageRepository pageRepository)
		{
			_pageRepository = pageRepository;
		}

		[Route("Rename")]
		[HttpPost]
		public async Task Rename(string oldTagName, string newTagName)
		{
			throw new NotImplementedException();
		}

		[Route("AllTags")]
		[HttpGet]
		public async Task<IEnumerable<TagViewModel>> AllTags()
		{
			throw new NotImplementedException();
		}

		[Route("FindPageWithTag")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> FindPageWithTag(string tag)
		{
			throw new NotImplementedException();
		}
	}
}