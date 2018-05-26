using System.Threading.Tasks;

namespace Roadkill.Api.Interfaces
{
	public interface IExportService
	{
		Task<string> ExportPagesToXml();
	}
}