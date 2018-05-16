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
	public class PagesControllerTests
	{
		private Mock<IPageRepository> _pageRepositoryMock;
		private Mock<IPageViewModelConverter> _viewModelCreatorMock;
		private PagesController _pagesController;
		private Fixture _fixture;

		public PagesControllerTests()
		{
			_fixture = new Fixture();

			_pageRepositoryMock = new Mock<IPageRepository>();
			_viewModelCreatorMock = new Mock<IPageViewModelConverter>();

			_pagesController = new PagesController(_pageRepositoryMock.Object, _viewModelCreatorMock.Object);
		}

		[Fact]
		public async Task Add()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task Update()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task Delete()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task GetById()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task AllPages_should_call_repository_and_converter()
		{
			// given
			IEnumerable<Page> pages = _fixture.CreateMany<Page>();

			_pageRepositoryMock.Setup(x => x.AllPages())
							   .ReturnsAsync(pages);

			// when
			IEnumerable<PageViewModel> pageViewModels = await _pagesController.AllPages();

			// then
			pageViewModels.Count().ShouldBe(pages.Count());

			_pageRepositoryMock.Verify(x => x.AllPages(), Times.Once);
			_viewModelCreatorMock.Verify(x => x.CreateViewModel(It.IsAny<Page>()));
		}

		[Fact]
		public async Task AllPagesCreatedBy_should_call_repository_and_converter()
		{
			// given
			string username = "bob";
			IEnumerable<Page> pages = _fixture.CreateMany<Page>();

			_pageRepositoryMock.Setup(x => x.FindPagesCreatedBy(username))
				.ReturnsAsync(pages);

			// when
			IEnumerable<PageViewModel> pageViewModels = await _pagesController.AllPagesCreatedBy(username);

			// then
			pageViewModels.Count().ShouldBe(pages.Count());

			_pageRepositoryMock.Verify(x => x.FindPagesCreatedBy(username), Times.Once);
			_viewModelCreatorMock.Verify(x => x.CreateViewModel(It.IsAny<Page>()));
		}

		[Fact]
		public async Task FindHomePage()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task FindByTitle()
		{
			// given

			// when

			// then
		}
	}
}