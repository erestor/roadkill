using System;
using System.Collections.Generic;
using Roadkill.Core.Text.Menu;

namespace Roadkill.Core.Text.TextMiddleware
{
    public class TextMiddlewareBuilder
    {
        public List<Middleware> MiddlewareItems { get; set; }

        public TextMiddlewareBuilder()
        {
            MiddlewareItems = new List<Middleware>();
        }

        public void Use(Middleware middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            MiddlewareItems.Add(middleware);
        }

        public PageHtml Execute(string markdown)
        {
            var pageHtml = new PageHtml() {Html = markdown};

            foreach (Middleware item in MiddlewareItems)
            {
                try
                {
                    pageHtml = item.Invoke(pageHtml);
                }
                catch (Exception ex)
                {
                    // TODO: logging
	                Console.WriteLine("------------------------------------------------");
                    Console.WriteLine("TextMiddlewareBuilder exception: {0}", ex);
                }
            }

            return pageHtml;
        }
    }
}
