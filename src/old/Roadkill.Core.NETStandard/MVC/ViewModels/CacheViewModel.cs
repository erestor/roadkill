using System.Collections.Generic;

namespace Roadkill.Core.Mvc.ViewModels
{
	public class CacheViewModel
	{
		public IEnumerable<string> PageKeys { get; set; }
		public IEnumerable<string> ListKeys { get; set; }
		public IEnumerable<string> SiteKeys { get; set; }
		public bool IsCacheEnabled { get; set; }
	}
}
