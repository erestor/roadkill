using System;
using System.Collections.Concurrent;
using Marten;
using Roadkill.Core.Models;

namespace Roadkill.Tests.Integration.Repositories
{
	public class DocumentStoreManager
	{
		private static readonly ConcurrentDictionary<string, IDocumentStore> _documentStores = new ConcurrentDictionary<string, IDocumentStore>();
		public static string ConnectionString => "host=localhost;port=5432;database=roadkill;username=roadkill;password=roadkill;";

		public static IDocumentStore GetMartenDocumentStore(Type testClassType)
		{
			string documentStoreSchemaName = "";

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
				options.CreateDatabasesForTenants(c =>
				{
					c.MaintenanceDatabase(connectionString);
					c.ForTenant()
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
					options.DatabaseSchemaName = schemaName;
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