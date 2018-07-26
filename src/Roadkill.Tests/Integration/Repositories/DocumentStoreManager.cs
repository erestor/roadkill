using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
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

			if (_documentStores.ContainsKey(documentStoreSchemaName))
			{
				outputHelper.WriteLine("GetMartenDocumentStore: found doc store in cache {0}", documentStoreSchemaName);
				return _documentStores[documentStoreSchemaName];
			}

			var keys = _documentStores.Select(x => x.Key);
			string debug = string.Join(",", keys);
			outputHelper.WriteLine("GetMartenDocumentStore: doc stores, items cached: {0}", debug);

			outputHelper.WriteLine("...creating one");
			IDocumentStore docStore = CreateDocumentStore(ConnectionString, documentStoreSchemaName, outputHelper);
			outputHelper.WriteLine("...creation done");
			_documentStores.AddOrUpdate(documentStoreSchemaName, docStore, (s, store) => store);

			return _documentStores[documentStoreSchemaName];
		}

		internal static IDocumentStore CreateDocumentStore(string connectionString, string schemaName, ITestOutputHelper outputHelper)
		{
			try
			{
				//Action<StoreOptions> options = ConfigureOptions(connectionString, schemaName, outputHelper);
				//outputHelper.WriteLine("...returning");
				//var documentStore = DocumentStore.For(options);

				StoreOptions options = ConfigureOptions(connectionString, schemaName, outputHelper);
				var documentStore = new DocumentStore(options);

				return documentStore;
			}
			catch (Exception e)
			{
				outputHelper.WriteLine("oops:");
				outputHelper.WriteLine(e.ToString());
				return null;
			}
		}

		private static StoreOptions ConfigureOptions(string connectionString, string schemaName, ITestOutputHelper outputHelper)
		{
			outputHelper.WriteLine("Configuring");
			//if (!string.IsNullOrEmpty(schemaName))
			schemaName = "roadkill";

			var options = new StoreOptions();

			//return options =>
			//{
			options.AutoCreateSchemaObjects = AutoCreate.All;
			//options.CreateDatabasesForTenants(c =>
			//{
			//	c.MaintenanceDatabase(connectionString);
			//	c.ForTenant(schemaName)
			//		.CheckAgainstPgDatabase()
			//		.WithOwner("postgres")
			//		.WithEncoding("UTF-8")
			//		.ConnectionLimit(-1)
			//		.OnDatabaseCreated(_ =>
			//		{
			//			outputHelper.WriteLine("GetMartenDocumentStore: Postgres 'roadkill' database created");
			//		});
			//});

			outputHelper.WriteLine("GetMartenDocumentStore: using '{0}' for schema name", schemaName);

			options.DatabaseSchemaName = schemaName;
			options.Connection(connectionString);
			options.Schema.For<User>().Index(x => x.Id);
			options.Schema.For<Page>().Identity(x => x.Id);
			options.Schema.For<Page>().Index(x => x.Id);
			options.Schema.For<PageVersion>().Index(x => x.Id);

			outputHelper.WriteLine("...done");
			//};

			return options;
		}
	}
}