using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using blogs_ASPNetCore.Models;
using CFHelper;

namespace blogs_ASPNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            EnvironmentVariables = new CFEnvironmentVariables(Configuration);
            Environment = env;
        }
        public IHostingEnvironment Environment { get; }
        public IConfigurationRoot Configuration { get; }
        public CFEnvironmentVariables EnvironmentVariables { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add our database context.
            // This will all DI to work and ensure that it is shared amongst other requesting classes.
            string connectionString = EnvironmentVariables.getConnectionStringForDbService("p-mysql", "mysql-service", new BasicMySQLConnectionStringFormatter());

            // If the connection string is empty, try to read in the configuration from appsettings.json
            if (connectionString == "")
            {
                var host = Configuration["MySql:hostname"];
                var username = Configuration["MySql:username"];
                var password = Configuration["MySql:password"];
                var databaseName = Configuration["MySql:database"];

                connectionString = new BasicMySQLConnectionStringFormatter().Format(host, username, password, databaseName);
            }

            services.AddDbContext<BloggingContext>(options => options.UseMySql(connectionString));

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Perform some database initialisation.
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<BloggingContext>();

                // Database.Migrate() will perform a migration of the database. This will ensure that the target database
                // is in sync with current context model snapshot found in the Migrations folder.
                // The alternative is to use EnsureCreated(). This will create a database and tables, if not existent on server.
                // However, consequently this skips migration altogether. Future migrations will not be possible
                // and one must issue EnsureDeleted() each time at the end to pull down the database.  
                // Therefore use EnsureCreated()/EnsureDeleted() for testing/development purposes only.
                // Note: On CF, it appears that EnsureDeleted may not work. For MySql dbs, it tries to 
                // access the 'mysql' database and fails because our random bound user (via cf bind-service) cannot 
                // this internal database.

                // For clarity and compatibility, we'll stick to Database.Migrate()
                // We do migrate here because potentially, one would need to initialise
                // a database on CF that may only be internal to CF or do further migrations in future. 
                // This will require a snapshot to be created first via dotnet ef migrations add <somename>.  
                // If access to CF database is not possible, point to a local database first.
                dbContext.Database.Migrate();

                if (env.IsDevelopment())
                {
                    // Ensure that we have some dummy data when we're performing development.
                    dbContext.EnsureSeedData();
                }
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=getBlogs}");
            });
        }
    }
}
