using Microsoft.AspNetCore.Authorization;
using Roadkill.Core.Configuration;
using Roadkill.Core.Services;
using StructureMap.Attributes;
using Roadkill.Core.DependencyResolution;
using Roadkill.Core.Security;

namespace Roadkill.Core.Mvc.Attributes
{
	/// <summary>
	/// Represents an attribute that is used to restrict access by callers to users that are in Editor role group.
	/// </summary>
	public class EditorRequiredAttribute : AuthorizeAttribute, ISetterInjected, IAuthorizationAttribute
	{
		[SetterProperty]
		public ApplicationSettings ApplicationSettings { get; set; }

		[SetterProperty]
		public IUserContext Context { get; set; }

		[SetterProperty]
		public UserServiceBase UserService { get; set; }

		[SetterProperty]
		public IPageService PageService { get; set; }

		[SetterProperty]
		public SettingsService SettingsService { get; set; }

		[SetterProperty]
		public IAuthorizationProvider AuthorizationProvider { get; set; }

		// TODO: NETStandard - figure out how this now works

		/// <summary>
		/// Provides an entry point for custom authorization checks.
		/// </summary>
		/// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
		/// <returns>
		/// true if the user is an editor, in the role name specified by the roadkill web.config editorRoleName setting or if this is blank; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext"/> parameter is null.</exception>
		//protected override bool AuthorizeCore(HttpContext httpContext)
		//{
		//	if (AuthorizationProvider == null)
		//		throw new SecurityException("The AuthorizationProvider property has not been set for EditorRequiredAttribute.", null);

		//	IPrincipal principal = httpContext.User;
		//	return AuthorizationProvider.IsEditor(principal);
		//}
	}
}