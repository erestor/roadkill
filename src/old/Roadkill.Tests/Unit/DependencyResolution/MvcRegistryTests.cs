using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using Roadkill.Core.Attachments;
using Roadkill.Core.Database;
using Roadkill.Core.DependencyResolution;
using Roadkill.Core.Mvc.Attributes;
using Roadkill.Core.Mvc.Controllers;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Mvc.WebApi;
using Roadkill.Core.Text.Parsers.Links;
using Roadkill.Tests.Unit.StubsAndMocks;
using Roadkill.Tests.Unit.StubsAndMocks.Mvc;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
	[TestFixture]
	[Category("Unit")]
	public class MvcRegistryTests : RegistryTestsBase
    {
		[Test]
		public void should_register_IRoadkillController_instances()
		{
			// Arrange
			IContainer container = Container;

			// Act
			IEnumerable<IRoadkillController> controllers = container.GetAllInstances<IRoadkillController>();

			// Assert
			Assert.That(controllers.Count(), Is.EqualTo(14));
		}

		[Test]
		public void should_register_ApiController_instances()
		{
			// Arrange
			IContainer container = Container;

			// Act
			IEnumerable<ApiController> controllers = container.GetAllInstances<ApiController>();

			// Assert
			Assert.That(controllers.Count(), Is.EqualTo(3));
		}

		[Test]
		public void should_register_ConfigurationTesterController_instance()
		{
			// Arrange
			IContainer container = Container;

			// Act
			IEnumerable<ConfigurationTesterController> controllers = container.GetAllInstances<ConfigurationTesterController>();

			// Assert
			Assert.That(controllers.Count(), Is.EqualTo(1));
		}

		[Test]
		public void should_fill_iauthorizationattribute_properties()
		{
			// Arrange
			IContainer container = Container;

			// Act
			IAuthorizationAttribute authorizationAttribute = container.GetInstance<AdminRequiredAttribute>();

			// Assert
			Assert.That(authorizationAttribute.AuthorizationProvider, Is.Not.Null);
		}

		[Test]
		public void should_register_default_MVC_models()
		{
			// Arrange
			IContainer container = Container;

			// Act
			UserViewModel userModel = container.GetInstance<UserViewModel>();
			SettingsViewModel settingsModel = container.GetInstance<SettingsViewModel>();
			AttachmentRouteHandler routerHandler = container.GetInstance<AttachmentRouteHandler>();

			// Assert
			Assert.That(userModel, Is.TypeOf<UserViewModel>());
			Assert.That(settingsModel, Is.TypeOf<SettingsViewModel>());
			Assert.That(routerHandler, Is.TypeOf<AttachmentRouteHandler>());
		}

		[Test]
		public void should_fill_isetterinjected_properties_for_attribute()
		{
			// Arrange
			IContainer container = Container;

			// Act
			ISetterInjected setterInjected = container.GetInstance<AdminRequiredAttribute>();

			// Assert
			Assert.That(setterInjected.ApplicationSettings, Is.Not.Null);
			Assert.That(setterInjected.Context, Is.Not.Null);
			Assert.That(setterInjected.UserService, Is.Not.Null);
			Assert.That(setterInjected.PageService, Is.Not.Null);
			Assert.That(setterInjected.SettingsService, Is.Not.Null);
		}

		[Test]
		public void should_fill_properties_for_ApiKeyAuthorizeAttribute()
		{
			// Arrange
			IContainer container = Container;

			// Act
			ApiKeyAuthorizeAttribute setterInjected = container.GetInstance<ApiKeyAuthorizeAttribute>();

			// Assert
			Assert.That(setterInjected.ApplicationSettings, Is.Not.Null);
		}

		[Test]
		public void should_use_route_table_for_routecollection()
		{
			// Arrange
			var route = new Route("test", new PageRouteHandler("~/index.html"));
			RouteTable.Routes.Add(route);
			IContainer container = Container;

			// Act
			var routeCollection = container.GetInstance<RouteCollection>();

			// Assert
			Assert.That(routeCollection, Is.Not.Null);
			Assert.That(routeCollection.Contains(route), Is.True);
		}

		[Test]
		public void should_inject_httpcontext_into_urlhelper_instance()
		{
			// Arrange
			var route = new Route("test", new PageRouteHandler("~/index.html"));
			RouteTable.Routes.Add(route);
			IContainer container = Container;

			var httpContext = MvcMockHelpers.FakeHttpContext("~/url");
			container.Configure(x => x.For<HttpContextBase>().Use(httpContext));

			// Act
			var urlHelper = container.GetInstance<UrlHelper>();

			// Assert
			Assert.That(urlHelper, Is.Not.Null);
		}
	}
}