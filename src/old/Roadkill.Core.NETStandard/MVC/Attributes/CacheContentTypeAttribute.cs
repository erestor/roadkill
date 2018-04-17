using System;

namespace Roadkill.Core.Mvc.Attributes
{
	/// <summary>
	/// Over-rides the OutputCache so it doesn't force text/html
	/// </summary>

	// TODO: NETStandard - use [Cache] instead
	public class CacheContentTypeAttribute : Attribute // : OutputCacheAttribute
	{
		//public string ContentType { get; set; }

		//public override void OnResultExecuted(ResultExecutedContext filterContext)
		//{
		//	base.OnResultExecuted(filterContext);

		//	ContentType = ContentType ?? "text/html";
		//	filterContext.HttpContext.Response.ContentType = ContentType;
		//}
	}
}