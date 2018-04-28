using Roadkill.Text.Text.Menu;
using Roadkill.Text.Text.Parsers;

namespace Roadkill.Text.Text.TextMiddleware
{
    public class MarkupParserMiddleware : Middleware
    {
        private IMarkupParser _parser;

        public MarkupParserMiddleware(IMarkupParser parser)
        {
            _parser = parser;
        }

        public override PageHtml Invoke(PageHtml pageHtml)
        {
            pageHtml.Html = _parser.ToHtml(pageHtml.Html);
            return pageHtml;
        }
    }
}