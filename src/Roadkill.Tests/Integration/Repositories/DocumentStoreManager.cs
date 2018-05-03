using System;
using Marten;
using Roadkill.Core.Models;

namespace Roadkill.Tests.Integration.Repositories
{
	public class DocumentStoreManager
	{
		private static IDocumentStore _martenDocumentStore;
		public static string ConnectionString => "host=localhost;port=5432;database=roadkill;username=roadkill;password=roadkill;";

		public static IDocumentStore MartenDocumentStore
		{
			get
			{
				if (_martenDocumentStore == null)
				{
					_martenDocumentStore = CreateDocumentStore(ConnectionString);
				}

				return _martenDocumentStore;
			}
		}

		internal static IDocumentStore CreateDocumentStore(string connectionString)
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

				options.Connection(connectionString);
				options.Schema.For<User>().Index(x => x.Id);
				options.Schema.For<Page>().Index(x => x.Id);
				options.Schema.For<PageContent>().Index(x => x.Id);
			});

			return documentStore;
		}
	}
}