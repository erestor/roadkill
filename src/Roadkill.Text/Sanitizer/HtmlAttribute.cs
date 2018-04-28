using System.Xml.Serialization;

namespace Roadkill.Text.Sanitizer
{
	public class HtmlAttribute
	{
		[XmlAttribute]
		public string Name { get; set; }

		public HtmlAttribute()
		{
		}

		public HtmlAttribute(string name)
		{
			Name = name;
		}
	}
}