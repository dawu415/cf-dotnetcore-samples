﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Pivotal Configuration Server Package
using Pivotal.Extensions.Configuration;

namespace Configuration
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddConfigServer(env)   // Read in the configuration server settings.  This must be after the appsettings.json.  
                                        // Our local configuration server settings are in the appsettings.json/appsettings.Development.json
                                        // Note that if the settings are not in the JSON files, it will attempt to read in the environment settings in env.
                                        // Or, if the settings are in the JSON files, the environment settings always takes precendence.
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set IConfigurationRoot to be an injectable dependency into other objects
            // In particular, this code uses the Configuration as a Singleton. An existing instance
            // that is shared amongst objects that will be using IConfigurationRoot. Object will remain
            // the same for every controller, service and request.
            // Alternatives: 
            //      - AddTransient< x > ()  : Objects are always different. A new object is always created for every controller and service.
            //      - AddScoped< x >()      : An instance of IConfigurationRoot that is created and released within a scope of a request. 
            //                                So within a request, the configuration object remains the same.  Between requests, the configuration object is different.
            services.AddSingleton<IConfigurationRoot>(Configuration);

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
