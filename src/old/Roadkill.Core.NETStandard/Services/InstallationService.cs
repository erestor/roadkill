using System;
using System.Collections.Generic;
using Roadkill.Core.Attachments;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Database.MongoDB;
using Roadkill.Core.Database.Repositories.Dapper;
using Roadkill.Core.Database.Schema;
using Roadkill.Core.DependencyResolution;
using Roadkill.Core.Mvc.ViewModels;
using StructureMap;

namespace Roadkill.Core.Services
{
	/// <summary>
	/// Provides common tasks for changing the Roadkill application settings.
	/// </summary>
	public class InstallationService : IInstallationService
	{
		private Func<string, string, IInstallerRepository> _getRepositoryFunc;
		internal IContainer Container { get; set; }

		public InstallationService()
		{
			_getRepositoryFunc = GetRepository;
			Container = LocatorStartup.Container;
		}

		internal InstallationService(Func<string, string, IInstallerRepository> getRepositoryFunc, IContainer container)
		{
			_getRepositoryFunc = getRepositoryFunc;
			Container = container;
		}

		public IEnumerable<RepositoryInfo> GetSupportedDatabases()
		{
			return new List<RepositoryInfo>()
			{
				SupportedDatabases.MongoDB,
				SupportedDatabases.MySQL,
				SupportedDatabases.Postgres,
				SupportedDatabases.SqlServer2008
			};
		}

		public void Install(SettingsViewModel model)
		{
			try
			{
				IInstallerRepository installerRepository = _getRepositoryFunc(model.DatabaseName, model.ConnectionString);
				installerRepository.CreateSchema();

				if (model.UseWindowsAuth == false)
				{
					installerRepository.AddAdminUser(model.AdminEmail, "admin", model.AdminPassword);
				}

				SiteSettings siteSettings = new SiteSettings();
				siteSettings.AllowedFileTypes = model.AllowedFileTypes;
				siteSettings.AllowUserSignup = model.AllowUserSignup;
				siteSettings.IsRecaptchaEnabled = model.IsRecaptchaEnabled;
				siteSettings.RecaptchaPrivateKey = model.RecaptchaPrivateKey;
				siteSettings.RecaptchaPublicKey = model.RecaptchaPublicKey;
				siteSettings.SiteUrl = model.SiteUrl;
				siteSettings.SiteName = model.SiteName;
				siteSettings.Theme = model.Theme;

				// v2.0
				siteSettings.OverwriteExistingFiles = model.OverwriteExistingFiles;
				siteSettings.HeadContent = model.HeadContent;
				siteSettings.MenuMarkup = model.MenuMarkup;
				installerRepository.SaveSettings(siteSettings);

				// Attachments handler needs re-registering
				var appSettings = Container.GetInstance<ApplicationSettings>();
				var fileService = Container.GetInstance<IFileService>();

				// TODO: NETStandard - update route handling
				//AttachmentRouteHandler.RegisterRoute(appSettings, RouteTable.Routes, fileService);
			}
			catch (DatabaseException ex)
			{
				throw new DatabaseException(ex, "An exception occurred while saving the site configuration.");
			}
		}

		internal IInstallerRepository GetRepository(string databaseName, string connectionString)
		{
			if (databaseName == SupportedDatabases.MongoDB)
			{
				return new MongoDbInstallerRepository(connectionString);
			}
			else if (databaseName == SupportedDatabases.MySQL)
			{
				throw new NotImplementedException();
			}
			else if (databaseName == SupportedDatabases.Postgres)
			{
				return new DapperInstallerRepository(new PostgresConnectionFactory(connectionString), new PostgresSchema());
			}
			else
			{
				return new DapperInstallerRepository(new SqlConnectionFactory(connectionString), new SqlServerSchema());
			}
		}
	}
}