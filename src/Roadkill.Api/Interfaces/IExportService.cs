using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roadkill.Api.Interfaces
{
	public interface IExportService
	{
		/// <summary>
		/// Exports all pages in the database, including content, to an XML format.
		/// </summary>
		/// <returns>An XML string.</returns>
		Task<string> ExportPagesToXml();
	}
}