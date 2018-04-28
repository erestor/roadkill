using Ganss.XSS;

namespace Roadkill.Text.Text.Sanitizer
{
	public interface IHtmlSanitizerFactory
	{
		IHtmlSanitizer CreateHtmlSanitizer();
	}
}