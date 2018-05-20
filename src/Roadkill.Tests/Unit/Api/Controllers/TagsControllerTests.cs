using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Roadkill.Api.Controllers;
using Roadkill.Api.Models;
using Roadkill.Core.Models;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Repositories;
using Shouldly;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration

namespace Roadkill.Tests.Unit.Api.Controllers
{
	public class TagsControllerTests
	{
		private Mock<IPageRepository> _pageRepositoryMock;
		private TagsController _tagsController;
		private Fixture _fixture;
		private Mock<IPageViewModelConverter> _pageViewModelConverterMock;

		public TagsControllerTests()
		{
			_fixture = new Fixture();

			_pageViewModelConverterMock = new Mock<IPageViewModelConverter>();
			_pageViewModelConverterMock
				.Setup(x => x.ConvertToViewModel(It.IsAny<Page>()))
				.Returns<Page>(page => new PageViewModel() { Id = page.Id, Title = page.Title });

			_pageRepositoryMock = new Mock<IPageRepository>();
			_tagsController = new TagsController(_pageRepositoryMock.Object, _pageViewModelConverterMock.Object);
		}

		[Fact]
		public async Task AllTags_should_return_all_tags_and_remove_duplicates()
		{
			// given
			List<string> tags = _fixture.CreateMany<string>().ToList();

			var duplicateTags = new List<string>();
			duplicateTags.Add("duplicate-tag");
			duplicateTags.Add("duplicate-tag");
			duplicateTags.Add("duplicate-tag");
			tags.AddRange(duplicateTags);

			int expectedTagCount = tags.Count - (duplicateTags.Count - 1);

			_pageRepositoryMock.Setup(x => x.AllTags())
				.ReturnsAsync(tags);

			// when
			IEnumerable<TagViewModel> tagViewModels = await _tagsController.AllTags();

			// then
			_pageRepositoryMock.Verify(x => x.AllTags(), Times.Once);
			tagViewModels.Count().ShouldBe(expectedTagCount);
		}

		[Fact]
		public async Task RenameTag()
		{
			// given
			IEnumerable<string> tags = _fixture.CreateMany<string>();

			//_pageRepositoryMock.Setup(x => x.tag(It.IsAny<string>(), It.IsAny<string>()))
			//	.Returns(Task.CompletedTask);

			// when
			await _tagsController.Rename("old tag", "new tag");

			// then
		}

		[Fact]
		public async Task FindByTag_should_use_repository_to_find_tags()
		{
			// given
			List<Page> pages = _fixture.CreateMany<Page>().ToList();
			pages[0].Tags += ", gutentag";
			pages[1].Tags += ", gutentag";
			pages[2].Tags += ", gutentag";

			_pageRepositoryMock
				.Setup(x => x.FindPagesContainingTag("gutentag"))
				.ReturnsAsync(pages);

			// when
			IEnumerable<PageViewModel> pageViewModelsWithTag = await _tagsController.FindPageWithTag("gutentag");

			// then
			pageViewModelsWithTag.Count().ShouldBe(pages.Count());

			_pageRepositoryMock.Verify(x => x.FindPagesContainingTag("gutentag"), Times.Once);
			_pageViewModelConverterMock.Verify(x => x.ConvertToViewModel(It.IsAny<Page>()));
		}
	}
}