// Don't change the namespace to "Roadkill.Core.Configuration" it will break older web.config files

using System;
using System.Collections.Generic;
using Roadkill.Core.Security;

namespace Roadkill.Core
{
	/// <summary>
	/// Config file settings - represents a &lt;roadkill&gt; section inside a configuration file.
	/// </summary>
	public class RoadkillSection
	{
		private Dictionary<string, string> _fakeDictionary = new Dictionary<string, string>();

		/// <summary>
		/// Gets or sets the name of the admin role.
		/// </summary>
		public string AdminRoleName
		{
			get { return (string)_fakeDictionary["adminRoleName"]; }
			set { _fakeDictionary["adminRoleName"] = value.ToString(); }
		}

		/// <summary>
		/// Gets or sets the api keys (comma seperated) used for access to the REST api. If this is empty, then the REST api is disabled.
		/// </summary>
		public string ApiKeys
		{
			get { return (string)_fakeDictionary["apiKeys"]; }
			set { _fakeDictionary["apiKeys"] = value.ToString(); }
		}

		/// <summary>
		/// Gets or sets the attachments folder, which should begin with "~/".
		/// </summary>
		public string AttachmentsFolder
		{
			get { return (string)_fakeDictionary["attachmentsFolder"]; }
			set { _fakeDictionary["attachmentsFolder"] = value.ToString(); }
		}

		/// <summary>
		/// TODO: comments
		/// </summary>
		public string AttachmentsRoutePath
		{
			get { return (string)_fakeDictionary["attachmentsRoutePath"]; }
			set { _fakeDictionary["attachmentsRoutePath"] = value.ToString(); }
		}

		/// <summary>
		/// Gets or sets the name of the connection string in the connectionstrings section.
		/// </summary>
		public string ConnectionStringName
		{
			get { return (string)_fakeDictionary["connectionStringName"]; }
			set { _fakeDictionary["connectionStringName"] = value.ToString(); }
		}

		/// <summary>
		/// Gets or sets the name of the editor role.
		/// </summary>
		public string EditorRoleName
		{
			get { return (string)_fakeDictionary["editorRoleName"]; }
			set { _fakeDictionary["editorRoleName"] = value.ToString(); }
		}

		/// <summary>
		/// Whether errors in updating the lucene index throw exceptions or are just ignored.
		/// </summary>
		public bool IgnoreSearchIndexErrors
		{
			get { return Convert.ToBoolean(_fakeDictionary["ignoreSearchIndexErrors"]); }
			set { _fakeDictionary["ignoreSearchIndexErrors"] = value.ToString(); }
		}

		/// <summary>
		/// Gets or sets whether this roadkill instance has been installed.
		/// </summary>
		public bool Installed
		{
			get { return Convert.ToBoolean(_fakeDictionary["installed"]); }
			set { _fakeDictionary["installed"] = value.ToString(); }
		}

		/// <summary>
		/// Whether the site is public, i.e. all pages are visible by default. The default is true,
		/// and this is optional.
		/// </summary>
		public bool IsPublicSite
		{
			get { return Convert.ToBoolean(_fakeDictionary["isPublicSite"]); }
			set { _fakeDictionary["isPublicSite"] = value.ToString(); }
		}

		/// <summary>
		/// For example: LDAP://mydc01.company.internal
		/// </summary>
		public string LdapConnectionString
		{
			get { return (string)_fakeDictionary["ldapConnectionString"]; }
			set { _fakeDictionary["ldapConnectionString"] = value.ToString(); }
		}

		/// <summary>
		/// The username to authenticate against the AD with
		/// </summary>
		public string LdapUsername
		{
			get { return (string)_fakeDictionary["ldapUsername"]; }
			set { _fakeDictionary["ldapUsername"] = value.ToString(); }
		}

		/// <summary>
		/// The password to authenticate against the AD with
		/// </summary>
		public string LdapPassword
		{
			get { return (string)_fakeDictionary["ldapPassword"]; }
			set { _fakeDictionary["ldapPassword"] = value.ToString(); }
		}

		/// <summary>
		/// Whether to remove all HTML tags from the markup except those found in the whitelist.xml file,
		/// inside the App_Data folder.
		/// </summary>
		public bool UseHtmlWhiteList
		{
			get { return Convert.ToBoolean(_fakeDictionary["useHtmlWhiteList"]); }
			set { _fakeDictionary["useHtmlWhiteList"] = value.ToString(); }
		}

		/// <summary>
		/// Whether to enabled Windows and Active Directory authentication.
		/// </summary>
		public bool UseWindowsAuthentication
		{
			get { return Convert.ToBoolean(_fakeDictionary["useWindowsAuthentication"]); }
			set { _fakeDictionary["useWindowsAuthentication"] = value.ToString(); }
		}

		/// <summary>
		/// The type used for the managing users, in the format "MyNamespace.Type".
		/// This class should inherit from the <see cref="UserServiceBase"/> class or a one of its derived types.
		/// </summary>
		public string UserServiceType
		{
			get { return (string)_fakeDictionary["userServiceType"]; }
			set { _fakeDictionary["userServiceType"] = value.ToString(); }
		}

		/// <summary>
		/// Indicates whether server-based page object caching is enabled.
		/// </summary>
		public bool UseObjectCache
		{
			get { return Convert.ToBoolean(_fakeDictionary["useObjectCache"]); }
			set { _fakeDictionary["useObjectCache"] = value.ToString(); }
		}

		/// <summary>
		/// Indicates whether page content should be cached, if <see cref="UseObjectCache"/> is true.
		/// </summary>
		public bool UseBrowserCache
		{
			get { return Convert.ToBoolean(_fakeDictionary["useBrowserCache"]); }
			set { _fakeDictionary["useBrowserCache"] = value.ToString(); }
		}

		/// <summary>
		/// Gets a value.ToString() indicating whether the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only,
		/// and can therefore be saved back to disk.
		/// </summary>
		/// <returns>This returns true.</returns>
		public bool IsReadOnly()
		{
			return false;
		}

		/// <summary>
		/// The database type for Roadkill. This defaults to SQLServer2008 (MongoDB on Mono) if empty.
		/// </summary>
		internal string DatabaseName
		{
			get { return (string)_fakeDictionary["databaseName"]; }
			set { _fakeDictionary["databaseName"] = value.ToString(); }
		}

		/// <summary>
		/// TODO: comments + tests
		/// </summary>
		public bool UseAzureFileStorage
		{
			get { return Convert.ToBoolean(_fakeDictionary["useAzureFileStorage"]); }
			set { _fakeDictionary["useAzureFileStorage"] = value.ToString(); }
		}

		/// <summary>
		/// TODO: comments + tests
		/// </summary>
		public string AzureConnectionString
		{
			get { return (string)_fakeDictionary["azureConnectionString"]; }
			set { _fakeDictionary["azureConnectionString"] = value.ToString(); }
		}

		/// <summary>
		/// TODO: comments + tests
		/// </summary>
		public string AzureContainer
		{
			get { return (string)_fakeDictionary["azureContainer"]; }
			set { _fakeDictionary["azureContainer"] = value.ToString(); }
		}
	}
}