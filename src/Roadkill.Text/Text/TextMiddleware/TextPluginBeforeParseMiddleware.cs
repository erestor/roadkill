using Roadkill.Core.Text.Menu;
using Roadkill.Core.Text.Plugins;

namespace Roadkill.Core.Text.TextMiddleware
{
    public class TextPluginBeforeParseMiddleware : Middleware
    {
        private readonly TextPluginRunner _textPluginRunner;

        public TextPluginBeforeParseMiddleware(TextPluginRunner textPluginRunner)
        {
            _textPluginRunner = textPluginRunner;
        }

        public override PageHtml Invoke(PageHtml pageHtml)
        {
            string text = _textPluginRunner.BeforeParse(pageHtml.Html, pageHtml);
            pageHtml.Html = text;

            return pageHtml;
        }
    }
}