using System;
using NUnit.Framework;
using Roadkill.Core.Database;
using Roadkill.Core.Database.MongoDB;
using Roadkill.Core.Database.Repositories;
using Roadkill.Core.Database.Repositories.Dapper;

namespace Roadkill.Tests.Unit.Database.Dapper
{
    public class RepositoryFactoryTests
    {
        [Test]
        public void should_return_null_when_connectionstring_is_null()
        {
            // Arrange
	        var factory = new RepositoryFactory();

			// Act
	        ISettingsRepository settingsRepo = factory.GetSettingsRepository(SupportedDatabases.SqlServer2008.Id, "");
			IUserRepository userRepo = factory.GetUserRepository(SupportedDatabases.SqlServer2008.Id, "");
			IPageRepository pageRepo = factory.GetPageRepository(SupportedDatabases.SqlServer2008.Id, "");

			// Assert
	        Assert.That(settingsRepo, Is.Null);
			Assert.That(userRepo, Is.Null);
			Assert.That(pageRepo, Is.Null);
		}

		[Test]
		[TestCase("SqlServer2008", typeof(SqlConnectionFactory))]
		[TestCase("Postgres", typeof(PostgresConnectionFactory))]
		public void should_return_dapper_settingsrepository_and_connectionfactory_for_databaseprovider(string databaseProvider, Type expectedType)
		{
			// Arrange
			var factory = new RepositoryFactory();

			// Act
			DapperSettingsRepository settingsRepo = factory.GetSettingsRepository(databaseProvider, "server=xyz") as DapperSettingsRepository;

			// Assert
			Assert.That(settingsRepo, Is.Not.Null);
			Assert.That(settingsRepo.DbConnectionFactory, Is.TypeOf(expectedType));
		}

		[Test]
		public void should_return_mongodb_settingsrepository()
		{
			// Arrange
			var factory = new RepositoryFactory();

			// Act
			MongoDBSettingsRepository settingsRepo = factory.GetSettingsRepository(SupportedDatabases.MongoDB.Id, "server=xyz") as MongoDBSettingsRepository;

			// Assert
			Assert.That(settingsRepo, Is.Not.Null);
		}

		[Test]
		[TestCase("SqlServer2008", typeof(SqlConnectionFactory))]
		[TestCase("Postgres", typeof(PostgresConnectionFactory))]
		public void should_return_dapper_userrepository_and_connectionfactory_for_databaseprovider(string databaseProvider, Type expectedType)
		{
			// Arrange
			var factory = new RepositoryFactory();

			// Act
			DapperUserRepository userRepository = factory.GetUserRepository(databaseProvider, "server=xyz") as DapperUserRepository;

			// Assert
			Assert.That(userRepository, Is.Not.Null);
			Assert.That(userRepository.DbConnectionFactory, Is.TypeOf(expectedType));
		}

		[Test]
		public void should_return_mongodb_userrepository()
		{
			// Arrange
			var factory = new RepositoryFactory();

			// Act
			MongoDBUserRepository userRepository = factory.GetUserRepository(SupportedDatabases.MongoDB.Id, "server=xyz") as MongoDBUserRepository;

			// Assert
			Assert.That(userRepository, Is.Not.Null);
		}

		[Test]
		[TestCase("SqlServer2008", typeof(SqlConnectionFactory))]
		[TestCase("Postgres", typeof(PostgresConnectionFactory))]
		public void should_return_dapper_pagerepository_and_connectionfactory_for_databaseprovider(string databaseProvider, Type expectedType)
		{
			// Arrange
			var factory = new RepositoryFactory();

			// Act
			DapperPageRepository pageRepository = factory.GetPageRepository(databaseProvider, "server=xyz") as DapperPageRepository;

			// Assert
			Assert.That(pageRepository, Is.Not.Null);
			Assert.That(pageRepository.DbConnectionFactory, Is.TypeOf(expectedType));
		}

		[Test]
		public void should_return_mongodb_pagerepository()
		{
			// Arrange
			var factory = new RepositoryFactory();

			// Act
			MongoDBPageRepository pageRepository = factory.GetPageRepository(SupportedDatabases.MongoDB.Id, "server=xyz") as MongoDBPageRepository;

			// Assert
			Assert.That(pageRepository, Is.Not.Null);
		}
	}
}