using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Roadkill.Core.Configuration;
using StructureMap.Attributes;

namespace Roadkill.Core.Mvc.WebApi
{
	public class ApiKeyAuthorizeAttribute : AuthorizeAttribute
	{
		[SetterProperty]
		public ApplicationSettings ApplicationSettings { get; set; }

		public static readonly string APIKEY_HEADER_KEY = "Authorization";

		// TODO: NETStandard - figure out how this works now
		//public override void OnAuthorization(HttpActionContext actionContext)
		//{
		//	if (!actionContext.Request.Headers.Contains(APIKEY_HEADER_KEY))
		//	{
		//		actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
		//		return;
		//	}

		//	string keyValue = actionContext.Request.Headers.GetValues(APIKEY_HEADER_KEY).First();

		//	if (!ApplicationSettings.ApiKeys.Contains(keyValue))
		//		actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
		//}
	}
}