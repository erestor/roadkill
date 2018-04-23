using Ganss.XSS;
using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.Sanitizer;

namespace Roadkill.Core.Text.TextMiddleware
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