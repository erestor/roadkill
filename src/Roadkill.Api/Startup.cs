using System;
using System.Reflection;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using Roadkill.Core.Models;

namespace Roadkill.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc(options => options.RespectBrowserAcceptHeader = true);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger(Assembly.GetEntryAssembly());
			app.UseSwaggerUi(Assembly.GetEntryAssembly());
			app.UseMvc();
		}

		public static string ConnectionString => "host=localhost;port=5432;database=roadkill;username=roadkill;password=roadkill;";
		private static IDocumentStore _documentStore;

		public static IDocumentStore GetMartenDocumentStore()
		{
			if (_documentStore == null)
			{
				_documentStore = CreateDocumentStore(ConnectionString);
			}

			return _documentStore;
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
				options.Schema.For<Page>().Identity(x => x.Id);
				options.Schema.For<Page>().Index(x => x.Id);
				options.Schema.For<PageVersion>().Index(x => x.Id);
			});

			return documentStore;
		}
	}
}