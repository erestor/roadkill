using System;
using System.IO;
using System.Text;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Roadkill.Core.Text.Parsers.Images;
using Roadkill.Core.Text.Parsers.Links;

namespace Roadkill.Core.Text.Parsers.Markdig
{
	public class MarkdigParser : IMarkupParser
	{
		public Func<HtmlImageTag, HtmlImageTag> ImageParsed { get; set; }
		public Func<HtmlLinkTag, HtmlLinkTag> LinkParsed { get; set; }

		public string ToHtml(string markdown)
		{
			if (string.IsNullOrEmpty(markdown))
				return "";

			var pipeline = new MarkdownPipelineBuilder();
			MarkdownPipeline markdownPipeline = pipeline.UseAdvancedExtensions().Build();

			MarkdownObject doc = Markdown.Parse(markdown, markdownPipeline);
			var walker = new MarkdigImageAndLinkWalker((e) =>
				{
					if (ImageParsed != null)
						ImageParsed(e);
				},
				(e) =>
				{
					if (LinkParsed != null)
						LinkParsed(e);
				});

			walker.WalkAndBindParseEvents(doc);

			var builder = new StringBuilder();
			var textwriter = new StringWriter(builder);

			var renderer = new HtmlRenderer(textwriter);
			renderer.Render(doc);

			return builder.ToString();
		}
	}
}