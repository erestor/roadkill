using System.Linq;
using Ganss.XSS;
using Microsoft.Extensions.Logging;

namespace Roadkill.Text.Text.Sanitizer
{
    public class HtmlSanitizerFactory : IHtmlSanitizerFactory
    {
        private readonly TextSettings _textSettings;
        private readonly ILogger _logger;

        public HtmlSanitizerFactory(TextSettings textSettings, ILogger logger)
        {
            _textSettings = textSettings;
            _logger = logger;
        }

        public IHtmlSanitizer CreateHtmlSanitizer()
        {
            if (!_textSettings.UseHtmlWhiteList)
                return null;

            HtmlWhiteList htmlWhiteList = HtmlWhiteList.Deserialize(_textSettings, _logger);
            string[] allowedTags = htmlWhiteList.ElementWhiteList.Select(x => x.Name).ToArray();
            string[] allowedAttributes =
                htmlWhiteList.ElementWhiteList.SelectMany(x => x.AllowedAttributes.Select(y => y.Name)).ToArray();

            if (allowedTags.Length == 0)
                allowedTags = null;

            if (allowedAttributes.Length == 0)
                allowedAttributes = null;

            var htmlSanitizer = new HtmlSanitizer(allowedTags, null, allowedAttributes);
            htmlSanitizer.AllowDataAttributes = false;
            htmlSanitizer.AllowedAttributes.Add("class");
            htmlSanitizer.AllowedAttributes.Add("id");
            htmlSanitizer.AllowedSchemes.Add("mailto");
            htmlSanitizer.RemovingAttribute += (sender, e) =>
            {
                // Don't clean /wiki/Special:Tag urls in href="" attributes
                if (e.Attribute.Name.ToLower() == "href" && e.Attribute.Value.Contains("Special:"))
                {
                    e.Cancel = true;
                }
            };

            return htmlSanitizer;
        }
    }
}