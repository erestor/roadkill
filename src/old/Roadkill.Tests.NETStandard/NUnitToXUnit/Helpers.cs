using Microsoft.Extensions.PlatformAbstractions;

namespace Roadkill.Tests.NETStandard.NUnitToXUnit
{
	public class Helpers
	{
		public static string GetCurrentDirectory()
		{
			return new ApplicationEnvironment().ApplicationBasePath;
		}
	}
}