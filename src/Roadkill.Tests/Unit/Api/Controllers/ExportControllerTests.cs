using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Roadkill.Api.Controllers;
using Roadkill.Core.Repositories;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration

namespace Roadkill.Tests.Unit.Api.Controllers
{
	public class ExportControllerTests
	{
		private Mock<IPageRepository> _pageRepositoryMock;
		private ExportController _exportController;
		private Fixture _fixture;

		public ExportControllerTests()
		{
			_fixture = new Fixture();

			_pageRepositoryMock = new Mock<IPageRepository>();
			_exportController = new ExportController(_pageRepositoryMock.Object);
		}

		[Fact]
		public async Task ExportToXml()
		{
			// given

			// when

			// then
		}
	}
}