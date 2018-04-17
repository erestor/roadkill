using System;
using System.Collections.Generic;

namespace Roadkill.Core.Security.Windows
{
	/// <summary>
	/// Default AD implentation of the provider.
	/// </summary>
	public class ActiveDirectoryProvider : IActiveDirectoryProvider
	{
		public IEnumerable<IPrincipalDetails> GetMembers(string domainName, string username, string password, string groupName)
		{
			throw new NotImplementedException();
		}

		public string TestLdapConnection(string connectionString, string username, string password, string groupName)
		{
			throw new NotImplementedException();
		}
	}
}