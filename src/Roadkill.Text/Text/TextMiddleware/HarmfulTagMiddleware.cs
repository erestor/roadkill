using Ganss.XSS;
using Roadkill.Text.Text.Menu;
using Roadkill.Text.Text.Sanitizer;

namespace Roadkill.Text.Text.TextMiddleware
{
	public class HarmfulTagMiddleware : Middleware
	{
		private readonly IHtmlSanitizer _sanitizer;

		public HarmfulTagMiddleware(IHtmlSanitizerFactory htmlSanitizerFactory)
		{
			_sanitizer = htmlSanitizerFactory.CreateHtmlSanitizer();
		}

		public override PageHtml Invoke(PageHtml pageHtml)
		{
			if (_sanitizer != null)
			{
				pageHtml.Html = _sanitizer.Sanitize(pageHtml.Html);
			}

			return pageHtml;
		}
	}
}