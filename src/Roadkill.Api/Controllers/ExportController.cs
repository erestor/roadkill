using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roadkill.Api.Interfaces;
using Roadkill.Core.Repositories;

namespace Roadkill.Api.Controllers
{
	[Route("export")]
	public class ExportController : Controller, IExportService
	{
		private readonly IPageRepository _pageRepository;

		public ExportController(IPageRepository pageRepository)
		{
			_pageRepository = pageRepository;
		}

		[Route("ExportToXml")]
		[HttpGet]
		public async Task<string> ExportToXml()
		{
			throw new NotImplementedException();
		}
	}
}