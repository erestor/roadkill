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
	public class PageVersionsControllerTests
	{
		private Mock<IPageRepository> _pageRepositoryMock;
		private Mock<IPageViewModelConverter> _viewModelCreatorMock;
		private PageVersionsController _pageVersionsController;
		private Fixture _fixture;

		public PageVersionsControllerTests()
		{
			_fixture = new Fixture();

			_pageRepositoryMock = new Mock<IPageRepository>();
			_viewModelCreatorMock = new Mock<IPageViewModelConverter>();

			_pageVersionsController = new PageVersionsController(_pageRepositoryMock.Object);
		}

		[Fact]
		public async Task GetLatestVersion()
		{
			// given

			// when

			// then
		}

		[Fact]
		public async Task UpdateLinksToPage()
		{
			// given

			// when

			// then
		}
	}
}