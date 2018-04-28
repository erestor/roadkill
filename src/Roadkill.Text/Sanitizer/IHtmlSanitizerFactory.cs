using Ganss.XSS;

namespace Roadkill.Text.Sanitizer
{
	public interface IHtmlSanitizerFactory
	{
		IHtmlSanitizer CreateHtmlSanitizer();
	}
}