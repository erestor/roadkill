using System.Web.Mvc;

namespace Roadkill.Tests.Unit.StubsAndMocks
{
	public class UrlHelperMock : UrlHelper
	{
		public bool ContentCalled { get; set; }
		public string ExpectedAction { get; set; }

		public override string Content(string path)
		{
			ContentCalled = true;
			return path;
		}

		public override string Action(string actionName, string controllerName, object routeValues)
		{
			return ExpectedAction;
		}
	}
}