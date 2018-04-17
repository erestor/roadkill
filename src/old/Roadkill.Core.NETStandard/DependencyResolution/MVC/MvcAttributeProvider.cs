using System.Collections.Generic;
using System.Linq;
using Roadkill.Core.Logging;
using StructureMap;

namespace Roadkill.Core.DependencyResolution.MVC
{
	/// <summary>
	/// The factory for all MVC attributes. This should be marked internal to avoid Structuremap issues.
	/// </summary>
	internal class MvcAttributeProvider // : FilterAttributeFilterProvider, IFilterProvider
	{
		// TODO: NETStandard - needs a complete rewrite for MVC 6

		//private readonly IContainer _container;
		//private readonly IEnumerable<IFilterProvider> _webApiProviders;

		//public MvcAttributeProvider(IContainer container)
		//{
		//	_container = container;
		//}

		//// For web api
		//public MvcAttributeProvider(IEnumerable<IFilterProvider> providers, IContainer container)
		//{
		//	_webApiProviders = providers;
		//	_container = container;
		//}

		//protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		//{
		//	IEnumerable<FilterAttribute> filters = base.GetControllerAttributes(controllerContext, actionDescriptor);

		//	foreach (FilterAttribute filter in filters)
		//	{
		//		_container.BuildUp(filter);
		//	}

		//	return filters;
		//}

		//protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		//{
		//	IEnumerable<FilterAttribute> filters = base.GetActionAttributes(controllerContext, actionDescriptor);

		//	foreach (FilterAttribute filter in filters)
		//	{
		//		_container.BuildUp(filter);
		//	}

		//	return filters;
		//}

		//public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		//{
		//	IEnumerable<Filter> filters = base.GetFilters(controllerContext, actionDescriptor);

		//	foreach (Filter filter in filters)
		//	{
		//		// Injects the instance with Structuremap's dependencies
		//		Log.Information(filter.Instance.GetType().Name);
		//		_container.BuildUp(filter.Instance);
		//	}

		//	return filters;
		//}

		//// WebApi

		//public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
		//{
		//	if (_webApiProviders != null)
		//	{
		//		IEnumerable<System.Web.Http.Filters.IFilterProvider> filterProviders = _webApiProviders;
		//		IEnumerable<System.Web.Http.Filters.FilterInfo> filters = filterProviders.SelectMany(x => x.GetFilters(configuration, actionDescriptor)).ToList();

		//		foreach (System.Web.Http.Filters.FilterInfo filter in filters)
		//		{
		//			// Injects the instance with Structuremap's dependencies
		//			Log.Information(filter.Instance.GetType().Name);
		//			_container.BuildUp(filter.Instance);
		//		}

		//		return filters;
		//	}
		//	else
		//	{
		//		// _webApiProviders will be null for something
		//		return null;
		//	}
		//}
	}
}