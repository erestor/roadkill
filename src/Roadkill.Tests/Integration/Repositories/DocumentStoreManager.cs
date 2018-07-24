using System;
using System.Collections.Concurrent;
using Marten;
using Roadkill.Core.Models;

namespace Roadkill.Tests.Integration.Repositories
{
	// docker run -d -p 9000:5432 --name roadkill-postgres -e POSTGRES_USER=roadkill -e POSTGRES_PASSWORD=roadkill postgres

	public class DocumentStoreManager
	{
		private static readonly ConcurrentDictionary<string, IDocumentStore> _documentStores = new ConcurrentDictionary<string, IDocumentStore>();
		public static string ConnectionString => "host=localhost;port=9000;database=roadkill;username=roadkill;password=roadkill;";

		public static IDocumentStore GetMartenDocumentStore(Type testClassType)
		{
			string documentStoreSchemaName = "";

			// Use a different schema for each test class, so their data is isolated
			if (testClassType != null)
				documentStoreSchemaName = testClassType.Name;

			if (!_documentStores.ContainsKey(documentStoreSchemaName))
			{
				IDocumentStore docStore = CreateDocumentStore(ConnectionString, documentStoreSchemaName);
				_documentStores.TryAdd(documentStoreSchemaName, docStore);
			}

			return _documentStores[documentStoreSchemaName];
		}

		internal static IDocumentStore CreateDocumentStore(string connectionString, string schemaName)
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
							Console.WriteLine("Postgres 'roadkill' database created");
						});
				});

				if (!string.IsNullOrEmpty(schemaName))
				{
					//options.DatabaseSchemaName = schemaName;
				}

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