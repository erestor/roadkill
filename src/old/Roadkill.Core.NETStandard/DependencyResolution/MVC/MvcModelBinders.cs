using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Roadkill.Core.Mvc.ViewModels;

namespace Roadkill.Core.DependencyResolution.MVC
{
	/// <summary>
	/// Used by the MVC framework to create all instances of a <see cref="UserViewModel"/> view model object.
	/// </summary>
	internal class UserViewModelModelBinder //: DefaultModelBinder
	{
		// TODO: NETStandard - needs a complete rewrite for MVC 6
		//protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
		//{
		//	return LocatorStartup.Container.Container.GetInstance<UserViewModel>();
		//}
	}

	/// <summary>
	/// Used by the MVC framework to create all instances of a <see cref="SettingsViewModel"/> view model object.
	/// </summary>
	internal class SettingsViewModelBinder // : DefaultModelBinder
	{
		// TODO: NETStandard - needs a complete rewrite for MVC 6
		//protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
		//{
		//	return LocatorStartup.Container.Container.GetInstance<SettingsViewModel>();
		//}
	}
}