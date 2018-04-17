using System.Security.Principal;

namespace Roadkill.Core.Security.Windows
{
	/// Wraps information needed from an <see cref="IPrincipal"/>
	/// </summary>
	public interface IPrincipalDetails
	{
		string SamAccountName { get; set; }
	}
}