namespace Roadkill.Core.Security
{
	/// <summary>
	/// Used to wrap FormsAuthentication methods, where Mono does not implement the methods or
	/// behaves slightly differently from the Windows implementation.
	/// </summary>
	public class FormsAuthenticationWrapper
	{
		public static bool IsEnabled()
		{
			return false;
		}

		public static string CookieName()
		{
			return "not implemented";
		}
	}
}