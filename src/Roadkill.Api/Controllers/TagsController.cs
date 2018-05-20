using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Api.Interfaces;
using Roadkill.Api.Models;
using Roadkill.Core.Models;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Repositories;

namespace Roadkill.Api.Controllers
{
	[Route("tags")]
	public class TagsController : Controller, ITagsService
	{
		private readonly IPageRepository _pageRepository;
		private IPageViewModelConverter _pageViewModelConverter;

		public TagsController(IPageRepository pageRepository, IPageViewModelConverter pageViewModelConverter)
		{
			_pageRepository = pageRepository;
			_pageViewModelConverter = pageViewModelConverter;
		}

		[Route("Rename")]
		[HttpPost]
		public async Task Rename(string oldTagName, string newTagName)
		{
			IEnumerable<Page> pages = await _pageRepository.FindPagesContainingTag(oldTagName);

			foreach (Page page in pages)
			{
				page.Tags = Regex.Replace(page.Tags, $@"\s{oldTagName}\s", newTagName);
				await _pageRepository.UpdateExisting(page);
			}
		}

		[Route("AllTags")]
		[HttpGet]
		public async Task<IEnumerable<TagViewModel>> AllTags()
		{
			IEnumerable<string> allTags = await _pageRepository.AllTags();

			return allTags
					.Distinct()
					.Select(tag => new TagViewModel(tag));
		}

		[Route("FindPageWithTag")]
		[HttpGet]
		public async Task<IEnumerable<PageViewModel>> FindPageWithTag(string tag)
		{
			IEnumerable<Page> pages = await _pageRepository.FindPagesContainingTag(tag);
			return pages.Select(_pageViewModelConverter.ConvertToViewModel);
		}
	}
}