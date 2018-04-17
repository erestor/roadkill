using NUnit.Framework;
using Roadkill.Core.Configuration;
using Roadkill.Core.Database;
using Roadkill.Core.Database.MongoDB;
using Roadkill.Core.Database.Repositories;
using Roadkill.Core.Database.Repositories.Dapper;
using Roadkill.Core.DependencyResolution.StructureMap.Registries;
using StructureMap;

namespace Roadkill.Tests.Unit.DependencyResolution
{
	[TestFixture]
	[Category("Unit")]
	public class RepositoryRegistryTests : RegistryTestsBase
    {
		[Test]
		public void should_use_dapperrepositories_by_default()
		{
			// Arrange + Act + Assert
			AssertDefaultType<ISettingsRepository, DapperSettingsRepository>();
			AssertDefaultType<IUserRepository, DapperUserRepository>();
			AssertDefaultType<IPageRepository, DapperPageRepository>();
		}

		[Test]
		public void should_load_repositoryfactory_by_default()
		{
			// Arrange + Act + Assert
			AssertDefaultType<IRepositoryFactory, RepositoryFactory>();
		}

		[Test]
		public void MongoDB_databaseType_should_load_repository()
		{
			// Arrange
			var settings = new ApplicationSettings();
			settings.DatabaseName = "MongoDB";
			settings.ConnectionString = "none empty connection string";
			InjectApplicationSettings(settings);

			var container = Container;

			// Act +  Assert
			AssertDefaultType<ISettingsRepository, MongoDBSettingsRepository>(container);
			AssertDefaultType<IUserRepository, MongoDBUserRepository>(container);
			AssertDefaultType<IPageRepository, MongoDBPageRepository>(container);
		}
	}
}
