using System;
using System.IO;
using System.Reflection;
using Marten.AspNetIdentity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;
using Roadkill.Core.Configuration;

namespace Roadkill.Api
{
	public class Startup
	{
		public IConfigurationRoot Configuration { get; set; }

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder();
			builder
				.SetBasePath(Path.Combine(env.ContentRootPath))
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging();

			string connectionString = Configuration["ConnectionString"];
			services.AddIdentity<RoadkillUser, IdentityRole>(options =>
				{
					options.Password.RequireDigit = true;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
				})
				.AddMartenStores<RoadkillUser, IdentityRole>()
				.AddDefaultTokenProviders();

			Roadkill.Core.DependencyInjection.ConfigureServices(services, connectionString);
			Roadkill.Api.DependencyInjection.ConfigureServices(services);

			services.AddOptions();
			services.Configure<SmtpSettings>(Configuration.GetSection("Smtp"));
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseExceptionHandler("/error");
			app.UseSwagger(Assembly.GetEntryAssembly());
			app.UseSwaggerUi(Assembly.GetEntryAssembly());
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseMvc();
		}
	}

	public class RoadkillUser : IdentityUser
	{
	}
}