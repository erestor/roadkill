using Roadkill.Core.Text.Menu;

namespace Roadkill.Core.Text.TextMiddleware
{
    public abstract class Middleware
    {
        public abstract PageHtml Invoke(PageHtml pageHtml);
    }
}