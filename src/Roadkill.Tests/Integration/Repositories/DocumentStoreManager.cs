using System;
using System.Collections.Concurrent;
using Marten;
using Roadkill.Core.Models;
using Xunit.Abstractions;

namespace Roadkill.Tests.Integration.Repositories
{
	// docker run -d -p 5432:5432 --name roadkill-postgres postgres
	// docker exec roadkill-postgres psql -c 'create database roadkill;' -U postgres

	public class DocumentStoreManager
	{
		private static readonly ConcurrentDictionary<string, IDocumentStore> _documentStores = new ConcurrentDictionary<string, IDocumentStore>();
		public static string ConnectionString => "host=localhost;port=5432;database=roadkill;username=postgres;password=;";

		public static IDocumentStore GetMartenDocumentStore(Type testClassType, ITestOutputHelper outputHelper)
		{
			string documentStoreSchemaName = "";

			// Use a different schema for each test class, so their data is isolated
			if (testClassType != null)
				documentStoreSchemaName = testClassType.Name;

			if (!_documentStores.ContainsKey(documentStoreSchemaName))
			{
				IDocumentStore docStore = CreateDocumentStore(ConnectionString, documentStoreSchemaName, outputHelper);
				_documentStores.TryAdd(documentStoreSchemaName, docStore);
				outputHelper.WriteLine("Setup: added {0} as a document store", documentStoreSchemaName);
			}

			return _documentStores[documentStoreSchemaName];
		}

		internal static IDocumentStore CreateDocumentStore(string connectionString, string schemaName, ITestOutputHelper outputHelper)
		{
			var documentStore = DocumentStore.For(options =>
			{
				options.AutoCreateSchemaObjects = AutoCreate.All;
				options.CreateDatabasesForTenants(c =>
				{
					c.MaintenanceDatabase(connectionString);
					c.ForTenant(schemaName)
						.CheckAgainstPgDatabase()
						.WithOwner("roadkill")
						.WithEncoding("UTF-8")
						.ConnectionLimit(-1)
						.OnDatabaseCreated(_ =>
						{
							outputHelper.WriteLine("Setup: Postgres 'roadkill' database created");
						});
				});

				if (!string.IsNullOrEmpty(schemaName))
				{
					options.DatabaseSchemaName = schemaName;
				}

				outputHelper.WriteLine("Setup: using '{0}' for schema name", schemaName);

				options.Connection(connectionString);
				options.Schema.For<User>().Index(x => x.Id);
				options.Schema.For<Page>().Identity(x => x.Id);
				options.Schema.For<Page>().Index(x => x.Id);
				options.Schema.For<PageVersion>().Index(x => x.Id);
			});

			return documentStore;
		}
	}
}