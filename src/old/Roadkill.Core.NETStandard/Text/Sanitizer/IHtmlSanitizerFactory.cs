using Ganss.XSS;

namespace Roadkill.Core.Text.Sanitizer
{
	public interface IHtmlSanitizerFactory
	{
		IHtmlSanitizer CreateHtmlSanitizer();
	}
}