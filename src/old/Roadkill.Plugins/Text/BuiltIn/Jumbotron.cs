using System.Text.RegularExpressions;
using Roadkill.Core.Plugins;
using Roadkill.Core.Text;
using Roadkill.Core.Text.TextMiddleware;

namespace Roadkill.Plugins.Text.BuiltIn
{
	public class Jumbotron : TextPlugin
	{
		internal static readonly string REGEX_STRING = @"\[\[\[jumbotron=(?'inner'.*?)\]\]\]";
		internal static readonly Regex COMPILED_REGEX = new Regex(REGEX_STRING, RegexOptions.Singleline | RegexOptions.Compiled);
		internal static readonly string HTMLTEMPLATE = @"<div id=""roadkill-jumbotron"" class=""jumbotron""><div id=""inner"">${inner}</div></div>";

		private string _preContainerHtml = "";

		public override string Id
		{
			get 
			{ 
				return "Jumbotron";	
			}
		}

		public override string Name
		{
			get
			{
				return "Jumbotron";
			}
		}

		public override string Description
		{
			get
			{
				return "Adds a giant image to the top of the page, with markdown overlayed ontop. Usage: [[[jumbotron=your markdown here]]] and ensure you have an image called 'jumbotron.jpg' in your attachments folder.";
			}
		}

		public override string Version
		{

			get
			{
				return "1.0";
			}
		}

		public Jumbotron()
		{
			_preContainerHtml = "";
		}

		public override string BeforeParse(string markupText)
		{
			// Check for the jumbotron token
			if (COMPILED_REGEX.IsMatch(markupText))
			{
				MatchCollection matches = COMPILED_REGEX.Matches(markupText);

				// All instances of the token
				if (matches.Count > 0)
				{
                    System.Text.RegularExpressions.Match match = matches[0];

					// Grab the markdown after the [[[jumbotron=..]]] and parse it,
					// and put it back in.
					string innerMarkDown = match.Groups["inner"].Value;

					// _preContainerHtml is returned later and it contains the HTML that lives 
					// outside the container, that this plugin provides.
					_preContainerHtml = HTMLTEMPLATE.Replace("${inner}", innerMarkDown);
				}
				
				// Remove the token from the markdown/creole
				markupText = Regex.Replace(markupText, REGEX_STRING, "", COMPILED_REGEX.Options);
			}

			return markupText;
		}

		public override string GetPreContainerHtml()
		{
			return _preContainerHtml;
		}

		public override string GetHeadContent()
		{
			return GetCssLink("jumbotron.css");
		}
	}
}