using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

// 1. Add Microsoft.Extensions.Configuration.Commandline Reference to project.
//      Use Visual Studio or the CLI Tool with command     ' dotnet add package Microsoft.Extensions.Configuration.Commandline '
// 2. Add inclusion of dependency: using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;

namespace CF_Base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 3. Instantiate ConfigurationBuilder and add command line arguments as part of the configuration
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config) // 4. Use the Configuration "config" from the ConfigurationBuilder.
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
