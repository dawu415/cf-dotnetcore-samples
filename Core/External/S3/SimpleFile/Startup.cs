using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Amazon Specific headers
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace SimpleFile
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
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Access keys should be retrieve securely from a secure store and region should be in configuration.
            // As this is an example application, we place them here for the convienience of learning.
            var accesskey       = "ACCESS_KEY";
            var secretaccesskey = "ACCESS_KEY_SECRET";
            var region          = RegionEndpoint.USEast1;
            
            services.AddTransient<IAmazonS3>(
                                                serviceProvider => new AmazonS3Client(
                                                                                        accesskey, 
                                                                                        secretaccesskey, 
                                                                                        region
                                                                                    )
                                            );

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
