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

		public TagsControllerTests()
		{
			_fixture = new Fixture();

			_pageRepositoryMock = new Mock<IPageRepository>();
			_tagsController = new TagsController(_pageRepositoryMock.Object);
		}

		[Fact]
		public async Task AllTags()
		{
			// given
			IEnumerable<string> tags = _fixture.CreateMany<string>();

			_pageRepositoryMock.Setup(x => x.AllTags())
				.ReturnsAsync(tags);

			// when
			IEnumerable<TagViewModel> tagViewModels = await _tagsController.AllTags();

			// then
			tagViewModels.Count().ShouldBe(tags.Count());

			_pageRepositoryMock.Verify(x => x.AllPages(), Times.Once);
			_viewModelCreatorMock.Verify(x => x.CreateViewModel(It.IsAny<Page>()));
		}

		[Fact]
		public async Task RenameTag()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task FindByTag()
		{
			// given
			IEnumerable<Page> pages = _fixture.CreateMany<Page>();

			_pageRepositoryMock.Setup(x => x.AllPages())
				.ReturnsAsync(pages);

			// when
			IEnumerable<PageViewModel> pageViewModels = await _tagsController.AllPages();

			// then
			pageViewModels.Count().ShouldBe(pages.Count());

			_pageRepositoryMock.Verify(x => x.AllPages(), Times.Once);
			_viewModelCreatorMock.Verify(x => x.CreateViewModel(It.IsAny<Page>()));
		}
	}
}